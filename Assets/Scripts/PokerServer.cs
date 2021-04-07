using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Mirror;
using UnityEngine;

public class PokerServer : NetworkBehaviour
{
    public PokerClient client;
    public const string GameId = "3a6cffb4-8856-42e6-a3ed-a7be70953518";
    
    [SyncVar] public GameState gameState;

    [SyncVar(hook = nameof(SyncSeatToUserId))]
    private string slotToUserIdSerialized;
    private bool waitingTimeoutToStartGame;

    // Game related variables
    [SyncVar] public decimal highestBet;
    [SyncVar] public decimal totalBets;
    private Dictionary<int, decimal> spotToBet;
    private List<int> spectators;
    private Dictionary<int, List<Card>> spotToCards;
    private List<Card> deck;
    private bool hasRaise;
    private int spotWhoRaised;
    public int turn;

    // Public Functions
    public void AddPlayer(string userId)
    {
        CmdAddPlayer(userId);
    }

    public void RemovePlayer(string userId)
    {
        CmdRemovePlayer(userId);
    }

    // Sync Var Functions
    private void SyncSeatToUserId(string oldValue, string newValue)
    {
        client.OnSeatToUserIdChange(oldValue, newValue);
    }

    private void SetGameState(Dictionary<int, string> dict)
    {
        // If not enough players, set the game state to waiting for players
        if (dict.Count < 2)
        {
            ResetGameVariables();
        }
        // If more than 1 player and waiting for players, set the game state to betting
        else if (gameState == GameState.WaitingForPlayers)
        {
            gameState = GameState.Ante;
        }
    }

    #region Commands

    
    [Command(requiresAuthority = false)]
    public void CmdCreateGameSession(string userId, NetworkConnectionToClient sender = null)
    {
        DataManager.CreateGameSession(sender.connectionId, LobbyInfo.GetInstance().serverId,userId,GameId);
    }

    [Command(requiresAuthority = false)]
    public void CmdExitGameSession(NetworkConnectionToClient sender = null)
    {
        DataManager.UpdateGameSessionTime(sender.connectionId, LobbyInfo.GetInstance().serverId, GameId);
    }
    
    [Command(requiresAuthority = false)]
    private void CmdRemovePlayer(string userId, NetworkConnectionToClient sender = null)
    {
        RemovePlayerInternal(userId);
    }

    private void RemovePlayerInternal(string userId)
    {
        var dict = JsonLibrary.DeserializeDictionaryIntString(slotToUserIdSerialized);

        for (var i = 0; i < PokerClient.MaxPlayers; i++)
        {
            if (!dict.ContainsKey(i) || dict[i] != userId) continue;
            dict.Remove(i);
        }

        SetGameState(dict);
        slotToUserIdSerialized = JsonLibrary.SerializeDictionaryIntString(dict);
    }

    [Command(requiresAuthority = false)]
    public void CmdNextAntePlayer(int spot, decimal betAmount)
    {
        if (spotToBet == null) spotToBet = new Dictionary<int, decimal>();
        spotToBet.Add(spot, betAmount);
        totalBets = spotToBet.Values.Sum();

        var dict = JsonLibrary.DeserializeDictionaryIntString(slotToUserIdSerialized);

        var possibleTurns = dict.Keys.Where(x => x > turn).ToList();
        if (possibleTurns.Any())
        {
            turn = possibleTurns.Min();
            client.ClientRPCTurnChange(turn, 0, gameState);
        }
        else
        {
            highestBet = 0;
            gameState = GameState.InitialBets;
            hasRaise = false;
            spotWhoRaised = -1;
            turn = GetFirstTurn(dict);
            DealCards(dict);
            client.ClientRPCTurnChange(turn, highestBet, gameState);
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdNextInitialBet(int spot, decimal betAmount)
    {
        var dict = JsonLibrary.DeserializeDictionaryIntString(slotToUserIdSerialized);
        var doneBetting = HandleBetting(dict, spot, betAmount);

        if (!doneBetting) return;

        gameState = GameState.SwitchingCards;
        turn = GetFirstTurn(dict);
        client.ClientRPCTurnChange(turn, 0, gameState);
    }

    [Command(requiresAuthority = false)]
    private void CmdAddPlayer(string userId, NetworkConnectionToClient sender = null)
    {
        var dict = JsonLibrary.DeserializeDictionaryIntString(slotToUserIdSerialized);

        for (var i = 0; i < PokerClient.MaxPlayers; i++)
        {
            if (dict.ContainsKey(i)) continue;
            dict.Add(i, userId);
            break;
        }

        slotToUserIdSerialized = JsonLibrary.SerializeDictionaryIntString(dict);

        if (!waitingTimeoutToStartGame)
        {
            TryStartGame();
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdChangeCards(int spot, List<int> cardsToChange)
    {
        var hand = spotToCards[spot];

        var newCards = deck.DealCards(cardsToChange.Count);

        int changePosition = 0;
        foreach (var newCard in newCards)
        {
            hand[cardsToChange[changePosition]] = newCard;
            changePosition++;
        }

        var spotToCardsList = new List<string>();

        foreach (var keyValue in spotToCards)
        {
            var entry = keyValue.Key + ",";
            foreach (var card in spotToCards[keyValue.Key])
            {
                entry += card.Rank + "|" + card.Suit + ",";
            }

            // Remove last comma
            entry = entry.Substring(0, entry.Length - 1);
            spotToCardsList.Add(entry);
        }

        client.ClientRPCCardsDealt(spotToCardsList);

        var dict = JsonLibrary.DeserializeDictionaryIntString(slotToUserIdSerialized);

        var possibleTurns = dict.Keys.Where(x => x > turn).ToList();

        if (possibleTurns.Any())
        {
            turn = possibleTurns.Min();
            client.ClientRPCTurnChange(turn, 0, gameState);
        }
        else
        {
            turn = GetFirstTurn(dict);
            gameState = GameState.FinalBetting;
            client.ClientRPCTurnChange(turn, highestBet, gameState);
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdNextFinalBet(int spot, decimal betAmount)
    {
        var dict = JsonLibrary.DeserializeDictionaryIntString(slotToUserIdSerialized);
        var doneBetting = HandleBetting(dict, spot, betAmount);

        if (!doneBetting) return;

        FinishGame();
    }

    [Command(requiresAuthority = false)]
    public void CmdFold(int spot)
    {
        if (!spectators.Contains(spot))
        {
            spectators.Add(spot);
        }
        
        var dict = JsonLibrary.DeserializeDictionaryIntString(slotToUserIdSerialized).Keys.Where(x => !spectators.Contains(x));

        if (dict.Count() > 1)
        {
            switch (gameState)
            {
                case GameState.Ante:
                    CmdNextAntePlayer(spot,0);
                    break;
                case GameState.InitialBets:
                    CmdNextInitialBet(spot,0);
                    break;
                case GameState.FinalBetting:
                    CmdNextFinalBet(spot,0);
                    break;
            }
        }
        else
        {
            FinishGame();
        }
    }

    #endregion

    private void TryStartGame()
    {
        ResetGameVariables();
        var dict = JsonLibrary.DeserializeDictionaryIntString(slotToUserIdSerialized);
        var oldGameState = gameState;
        SetGameState(dict);

        if (oldGameState == GameState.WaitingForPlayers && gameState == GameState.Ante)
        {
            turn = GetFirstTurn(dict);
            client.ClientRPCTurnChange(turn, highestBet, gameState);
        }
        else if (gameState == GameState.WaitingForPlayers)
        {
            turn = -1;
        }

        waitingTimeoutToStartGame = false;
    }
    
    private bool HandleBetting(Dictionary<int, string> dict, int spot, decimal betAmount)
    {
        var isDone = false;
        if (spotToBet == null) spotToBet = new Dictionary<int, decimal>();
        spotToBet[spot] += betAmount;
        if (spotToBet[spot] > highestBet)
        {
            highestBet = spotToBet[spot];
            hasRaise = true;
            spotWhoRaised = spot;
        }
        
        totalBets = spotToBet.Values.Sum();

        var possibleTurns = dict.Keys.Where(x => x > turn && !spectators.Contains(x)).ToList();

        if (possibleTurns.Any())
        {
            turn = possibleTurns.Min();
        }
        else if (hasRaise)
        {
            turn = GetFirstTurn(dict);
            hasRaise = false;
        }
        else
        {
            isDone = true;
        }

        if (!isDone)
        {
            if (turn != spotWhoRaised)
            {
                client.ClientRPCTurnChange(turn, highestBet, gameState);
            }
            else
            {
                spotWhoRaised = -1;
                isDone = true;
            }
        }

        return isDone;
    }

    private void DealCards(Dictionary<int, string> dict)
    {
        spotToCards = new Dictionary<int, List<Card>>();
        deck = Deck.GetShuffledDeck(1);

        foreach (var keyValue in dict)
        {
            var hand = deck.DealCards(5);
            spotToCards.Add(keyValue.Key, hand);
        }

        var spotToCardsList = new List<string>();

        foreach (var keyValue in spotToCards)
        {
            var entry = keyValue.Key + ",";
            entry = spotToCards[keyValue.Key]
                .Aggregate(entry, (current, card) => current + (card.Rank + "|" + card.Suit + ","));

            // Remove last comma
            entry = entry.Substring(0, entry.Length - 1);
            spotToCardsList.Add(entry);
        }

        client.ClientRPCCardsDealt(spotToCardsList);
    }

    private int GetFirstTurn(Dictionary<int, string> dict)
    {
        var firstTurn = -1;
        var possibleTurns = dict.Keys.Where(key=> !spectators.Contains(key)).ToList();
        if (possibleTurns.Any())
        {
            firstTurn = possibleTurns.Min();
        }

        return firstTurn;
    }

    private void ResetGameVariables()
    {
        highestBet = 0;
        totalBets = 0;
        gameState = GameState.WaitingForPlayers;
        spotToCards = new Dictionary<int, List<Card>>();
        spotToBet = new Dictionary<int, decimal>();
        deck = null;
        spotWhoRaised = -1;
        hasRaise = false;
        turn = -1;
        spectators = new List<int>();
    }

    private void FinishGame()
    {
        gameState = GameState.Complete;

        var spotToHands =
            spotToCards.Where(keyValue => !spectators.Contains(keyValue.Key)).ToDictionary(keyValue => keyValue.Key, keyValue => keyValue.Value.GetEvaluatedHand());

        if (spotToHands.Any())
        {
            var spotToWinner = Deck.GetSpotToWinner(spotToHands);

            var winnerHand = PokerHand.HighCard;
            var numberOfWinners = spotToWinner.Values.Count(x => x);
            var totalAmount = spotToBet.Values.Sum();
            var winnerPortion = totalAmount / numberOfWinners;

            foreach (var keyValue in spotToWinner.Where(keyValue => keyValue.Value))
            {
                winnerHand = spotToHands[keyValue.Key].PokerHand;
                client.ClientRPCGameResult("You Won " + spotToHands[keyValue.Key].PokerHand, keyValue.Key, true,
                    winnerPortion);
                numberOfWinners++;
            }

            foreach (var keyValue in spotToWinner.Where(keyValue => !keyValue.Value))
            {
                client.ClientRPCGameResult($"You Lost :( Winner Had {winnerHand} ", keyValue.Key, false,
                    -spotToBet[keyValue.Key]);
            }
        }

        waitingTimeoutToStartGame = true;
        Invoke(nameof(TryStartGame), 5);
    }
}