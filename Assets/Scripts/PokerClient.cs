using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class PokerClient : NetworkBehaviour
{
    public List<PokerCardScript> pokerCardScripts;
    public GameObject cardGameObject;
    public TextMesh tableInfoTextMesh;
    public TextMesh sessionInfoTextMesh;
    public Text gameInfoText;

    #region Buttons

    public GameObject anteButton;
    public GameObject switchButton;
    public GameObject foldButton;
    public GameObject raiseButton;
    public GameObject checkButton;
    public GameObject callButton;
    public Text callButtonText;
    public GameObject raiseInputField;
    public Text raiseInputText;
    public Text raiseButtonText;

    #endregion

    public const int MaxPlayers = 3;
    public const int MaxBet = 250;
    private const int Ante = 100;
    public PokerServer server;
    public GameObject gameCamera;
    private int betAmount;
    private Dictionary<int, string> slotToUserId = new Dictionary<int, string>();

    // Local Values
    private Session session;

    private void Update()
    {
        tableInfoTextMesh.text = "Total Players: " + GetTotalPlayers() + "/" + MaxPlayers + "\n" +
                                 "Total Bets: $" + server.totalBets;

        sessionInfoTextMesh.text = session != null ? session.ToString() : "";
    }

    #region Event Listeners

    private void OnEnable()
    {
        EventManager.OnClick += OnClick;
        EventManager.OnReadyToGame += OnReadyToGame;
        EventManager.OnPrepareToExitGame += OnPrepareToExitGame;
        EventManager.OnModifyBetAction += OnModifyBetAction;
        EventManager.OnStartGame += OnStartGame;
    }

    private void OnDisable()
    {
        EventManager.OnClick -= OnClick;
        EventManager.OnReadyToGame -= OnReadyToGame;
        EventManager.OnPrepareToExitGame -= OnPrepareToExitGame;
        EventManager.OnModifyBetAction -= OnModifyBetAction;
        EventManager.OnStartGame -= OnStartGame;
    }

    #endregion

    #region Enter and Exit Game

    private void OnReadyToGame(int instanceId)
    {
        if (instanceId != transform.GetInstanceID()) return;

        DisableButtons();

        gameCamera.SetActive(true);
        cardGameObject.SetActive(false);
        EventManager.FireStartGameEvent(instanceId);
    }

    private void OnStartGame(int instanceId)
    {
        if (!IsTableFull())
        {
            server.AddPlayer(UserInfo.GetInstance().UserId);
            server.CmdCreateGameSession(UserInfo.GetInstance().UserId);
        }

        // TODO: Let the player know the table is full
    }

    private void OnPrepareToExitGame(int instanceId)
    {
        if (instanceId != transform.GetInstanceID()) return;

        gameCamera.SetActive(false);
        cardGameObject.SetActive(false);
        server.RemovePlayer(UserInfo.GetInstance().UserId);
        session = null;
        EventManager.FireInstructionChangeEvent("");
        EventManager.FireReadyToExitGameEvent(instanceId);
        server.CmdExitGameSession();
    }

    #endregion

    #region OnClicks

    public void OnClickAnteButton()
    {
        session.totalBets = Ante;
        EventManager.FireChangeCoinValue(-Ante);
        DataManager.ChangeCoinValue(UserInfo.GetInstance().UserId, -Ante);
        server.CmdNextAntePlayer(session.spot, Ante);
        DisableButtons();
    }

    public void OnClickFoldButton()
    {
        server.CmdFold(session.spot);
        cardGameObject.SetActive(false);
        session = null;
        DisableButtons();
    }

    public void OnClickRaiseButton()
    {
        if (!decimal.TryParse(raiseInputText.text, out var amount)) return;

        var minimumRaise = server.highestBet - session.totalBets;
        if (UserInfo.GetInstance().TotalCoins > amount && amount >= minimumRaise)
        {
            session.totalBets += amount;
            HandleBet(amount);
            DisableButtons();
        }
    }

    public void OnClickCallButton()
    {
        var amount = server.highestBet - session.totalBets;
        session.totalBets += amount;

        HandleBet(amount);
        DisableButtons();
    }

    public void OnClickCheckButton()
    {
        HandleBet(0);
        DisableButtons();
    }

    public void OnClickSwitchButton()
    {
        var cardsToChange = new List<int>();

        foreach (var script in pokerCardScripts)
        {
            script.disabled = true;
            if (script.changeRequested)
            {
                cardsToChange.Add(script.position);
            }
        }

        DisableButtons();
        server.CmdChangeCards(session.spot, cardsToChange);
    }

    #endregion

    private void HandleBet(decimal amount)
    {
        EventManager.FireChangeCoinValue(-amount);
        DataManager.ChangeCoinValue(UserInfo.GetInstance().UserId, -amount);

        switch (server.gameState)
        {
            case GameState.InitialBets:
                server.CmdNextInitialBet(session.spot, amount);
                break;
            case GameState.FinalBetting:
                server.CmdNextFinalBet(session.spot, amount);
                break;
        }
    }

    private void OnModifyBetAction(int amount)
    {
        if (server.gameState != GameState.Ante) return;
        session.totalBets = Ante;
    }

    private void OnClick(int instanceId)
    {
        if (instanceId != transform.GetInstanceID()) return;

        if (UserInfo.GetInstance().IsInGame()) return;

        if (UserInfo.GetInstance().UserId != null)
        {
            EventManager.FirePrepareToGameEvent(instanceId);
        }
    }

    public void OnSeatToUserIdChange(string oldValue, string newValue)
    {
        slotToUserId = JsonLibrary.DeserializeDictionaryIntString(newValue);
        var slotToUserIdOld =
            oldValue == null ? new Dictionary<int, string>() : JsonLibrary.DeserializeDictionaryIntString(oldValue);

        // If player has been added, set the seat number
        if (slotToUserId.Count > slotToUserIdOld.Count)
        {
            var newUserId =
                (from keyValue in slotToUserId
                    where !slotToUserIdOld.ContainsKey(keyValue.Key)
                    select keyValue.Value).FirstOrDefault();

            if (newUserId == null)
            {
                Debug.Log("Failed to find new player in slotToUserIdOld");
            }

            if (UserInfo.GetInstance().UserId != newUserId) return;

            if (session == null) return;

            session.spot = GetPlayerSeatNumber();
        }
    }

    [ClientRpc]
    public void ClientRPCCardsDealt(List<string> spotToCards)
    {
        var spot = session.spot;

        foreach (var str in spotToCards)
        {
            var strChunks = str.Split(',');
            if (int.Parse(strChunks[0]) == spot)
            {
                session.hand = str;
                cardGameObject.SetActive(true);

                for (var i = 1; i < strChunks.Length; i++)
                {
                    pokerCardScripts[i - 1].SetCardValue(strChunks[i]);
                    pokerCardScripts[i - 1].position = i - 1;
                    pokerCardScripts[i - 1].changeRequested = false;
                    pokerCardScripts[i - 1].disabled = true;
                }
            }
        }
    }

    [ClientRpc]
    public void ClientRPCTurnChange(int turn, decimal highestBet, GameState gameState)
    {
        // If the player left or it's not their turn, return
        var isNotUserTurn = !slotToUserId.ContainsKey(turn) || UserInfo.GetInstance().UserId != slotToUserId[turn];
        if (isNotUserTurn)
        {
            switch (gameState)
            {
                case GameState.Ante:
                    gameInfoText.text = $"Player #{turn + 1} Is Placing Their Ante";
                    break;
                case GameState.InitialBets:
                case GameState.FinalBetting:
                    gameInfoText.text = $"Player #{turn + 1} is Placing Their Bets";
                    break;
                case GameState.SwitchingCards:
                    gameInfoText.text = $"Player #{turn + 1} is Switching Their Cards";
                    break;
            }
        }
        else
        {
            switch (gameState)
            {
                case GameState.Ante:
                    session = new Session();
                    session.spot = GetPlayerSeatNumber();
                    anteButton.SetActive(true);
                    foldButton.SetActive(true);
                    gameInfoText.text = "Place Your Ante";
                    break;
                case GameState.InitialBets:
                case GameState.FinalBetting:
                    gameInfoText.text = "Place Your Bet";
                    var hasRaised = highestBet > session.totalBets;
                    checkButton.SetActive(!hasRaised);
                    callButton.SetActive(hasRaised);
                    raiseButton.SetActive(true);
                    raiseInputField.SetActive(true);
                    callButtonText.text = "Call $" + (highestBet - session.totalBets);
                    if (hasRaised)
                    {
                        raiseButtonText.text = "Raise Min. ($" + (highestBet - session.totalBets) + ")";
                        raiseInputText.text = (highestBet - session.totalBets) * 2 + "";
                    }
                    else
                    {
                        raiseButtonText.text = "Raise";
                        raiseInputText.text = "1";
                    }

                    foldButton.SetActive(true);
                    break;
                case GameState.SwitchingCards:
                    gameInfoText.text = $"Select Cards and Switch";
                    switchButton.SetActive(true);
                    foreach (var cardScript in pokerCardScripts)
                    {
                        cardScript.disabled = false;
                    }

                    break;
            }
        }
    }


    [ClientRpc]
    public void ClientRPCGameResult(string message, int turn, bool hasWon, decimal amount)
    {
        if (!slotToUserId.ContainsKey(turn) || UserInfo.GetInstance().UserId != slotToUserId[turn]) return;

        gameInfoText.text = message;
        if (hasWon)
        {
            EventManager.FireChangeCoinValue(amount);
        }
        else
        {
            EventManager.FireChangeCoinValue(amount);
        }
        
        DataManager.ChangeCoinValue(UserInfo.GetInstance().UserId, amount);
    }

    private bool IsTableFull()
    {
        return GetTotalPlayers() >= MaxPlayers;
    }

    private int GetTotalPlayers()
    {
        return slotToUserId?.Count ?? 0;
    }

    private int GetPlayerSeatNumber()
    {
        var slot = -1;
        foreach (var value in slotToUserId.Where(value => value.Value == UserInfo.GetInstance().UserId))
        {
            slot = value.Key;
            break;
        }

        return slot;
    }

    private void DisableButtons()
    {
        callButton.SetActive(false);
        foldButton.SetActive(false);
        raiseButton.SetActive(false);
        checkButton.SetActive(false);
        anteButton.SetActive(false);
        raiseInputField.SetActive(false);
        switchButton.SetActive(false);
        raiseInputText.text = "";
    }

    public class Session
    {
        public decimal totalBets;
        public int spot;
        public string hand;

        public Session()
        {
            spot = -1;
            totalBets = 0;
        }

        public override string ToString()
        {
            var str = "Your Total Bets: $" + totalBets + "\n" +
                      "Your Seat #: " + (spot + 1) + "\n";

            return str;
        }
    }
}