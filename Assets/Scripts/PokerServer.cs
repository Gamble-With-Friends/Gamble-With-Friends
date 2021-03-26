using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

public class PokerServer : NetworkBehaviour
{
    public PokerClient client;

    [SyncVar] public decimal totalBets;

    [SyncVar(hook = nameof(SyncGameState))]
    public GameState gameState;

    [SyncVar(hook = nameof(SyncSeatToUserId))]
    private string slotToUserIdSerialized;

    [SyncVar(hook = nameof(SyncTurn))] public int turn;

    private Dictionary<int, decimal> slotToBet;
    private Dictionary<int, List<Card>> spotToCards;

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

    private void SyncGameState(GameState oldValue, GameState newValue)
    {
        client.OnGameStateChange(oldValue.ToString(), newValue.ToString());
    }

    private void SyncTurn(int oldValue, int newValue)
    {
        client.OnTurnChange(newValue);
    }

    private void SetGameState(Dictionary<int, string> dict)
    {
        // If not enough players, set the game state to waiting for players
        if (dict.Count < 2)
        {
            gameState = GameState.WaitingForPlayers;
        }
        // If more than 1 player and waiting for players, set the game state to betting
        else if (gameState == GameState.WaitingForPlayers)
        {
            gameState = GameState.BettingAnte;
        }
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

        var oldGameState = gameState;
        SetGameState(dict);

        slotToUserIdSerialized = JsonLibrary.SerializeDictionaryIntString(dict);

        if (oldGameState == GameState.WaitingForPlayers && gameState == GameState.BettingAnte)
        {
            turn = 0;
        }
        else if (gameState == GameState.WaitingForPlayers)
        {
            turn = -1;
        }
    }

    [Command(requiresAuthority = false)]
    private void CmdRemovePlayer(string userId, NetworkConnectionToClient sender = null)
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
    public void CmdNextPlayer(int spot, decimal betAmount)
    {
        if (slotToBet == null) slotToBet = new Dictionary<int, decimal>();
        slotToBet.Add(spot, betAmount);

        var dict = JsonLibrary.DeserializeDictionaryIntString(slotToUserIdSerialized);

        var hasMoreTurns = false;
        foreach (var keyValue in dict.Where(keyValue => keyValue.Key > turn))
        {
            turn = keyValue.Key;
            hasMoreTurns = true;
            break;
        }

        if (!hasMoreTurns)
        {
            DealCards(dict);
        }
    }

    private void DealCards(Dictionary<int, string> dict)
    {
        spotToCards = new Dictionary<int, List<Card>>();
        var cards = Deck.GetShuffledDeck(1);

        foreach (var keyValue in dict)
        {
            var hand = cards.DealCards(5);
            spotToCards.Add(keyValue.Key, hand);
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
    }

    [Command(requiresAuthority = false)]
    public void CmdSetGameState(GameState state)
    {
        gameState = state;
    }
}