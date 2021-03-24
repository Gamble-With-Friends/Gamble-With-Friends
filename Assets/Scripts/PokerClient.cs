using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

public class PokerClient : NetworkBehaviour
{
    public List<PokerCardScript> pokerCardScripts;
    public GameObject cardGameObject;
    public GameObject chipsGameObject;
    public TextMesh totalPlayersTextMesh;
    public TextMesh totalBetsTextMesh;
    public TextMesh gameStatusTextMesh;
    public TextMesh sessionInfoTextMesh;
    public TextMesh debugTextMesh;

    public const int MaxPlayers = 2;
    public const int MaxBet = 250;
    public PokerServer server;
    public GameObject gameCamera;
    private int betAmount;
    private Dictionary<int, string> slotToUserId = new Dictionary<int, string>();
    private Dictionary<int, decimal> slotToBet = new Dictionary<int, decimal>();

    // Local Values
    private Session session;

    //GameObject variableForPrefab = (GameObject)Resources.Load("Prefabs/FirstPersonPlayer", typeof(GameObject));

    private void Update()
    {
        totalPlayersTextMesh.text = "Total Players: " + GetTotalPlayers() + "/" + MaxPlayers;
        totalBetsTextMesh.text = "Total Bets: $" + server.totalBets;
        gameStatusTextMesh.text = "Game Status: " + server.gameState;
        sessionInfoTextMesh.text = session != null ? session.ToString() : "";
        var debugText = "";
        foreach (var keyValue in slotToUserId)
        {
            debugText += " " + keyValue.Key + " => " + keyValue.Value + "\n";
        }

        debugTextMesh.text = debugText;
    }

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

    private void OnReadyToGame(int instanceId)
    {
        if (instanceId != transform.GetInstanceID()) return;

        session = new Session();
        gameCamera.SetActive(true);
        EventManager.FireStartGameEvent(instanceId);
    }

    private void OnStartGame(int instanceId)
    {
        if (!IsTableFull())
        {
            server.AddPlayer(UserInfo.GetInstance().UserId);
        }

        // TODO: Let the player know the table is full
    }

    private void OnModifyBetAction(int amount)
    {
        if (server.gameState != GameState.Betting) return;
        session.totalBets += amount;
        if (session.totalBets < 0)
        {
            session.totalBets = 0;
        }
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

    public void OnGameStateChange(string oldValue, string newValue)
    {
        var gameStateOld = (GameState) Enum.Parse(typeof(GameState), oldValue);
        var gameStateNew = (GameState) Enum.Parse(typeof(GameState), newValue);
    }

    public void OnTurnChange(int turn)
    {
        if (server.gameState == GameState.Betting)
        {
            if (session == null) return;
            var isPlayersTurn = UserInfo.GetInstance().UserId == slotToUserId[session.seatNumber];
        
            if (isPlayersTurn)
            {
                chipsGameObject.SetActive(true);
            }
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

            session.seatNumber = GetPlayerSeatNumber();
        }
    }

    private void OnPrepareToExitGame(int instanceId)
    {
        if (instanceId != transform.GetInstanceID()) return;
        gameCamera.SetActive(false);
        cardGameObject.SetActive(false);
        chipsGameObject.SetActive(false);
        server.RemovePlayer(UserInfo.GetInstance().UserId);
        session = null;
        if (slotToUserId.Count == 0)
        {
            server.CmdSetGameState(GameState.WaitingForPlayers);
        }

        EventManager.FireInstructionChangeEvent("");
        EventManager.FireReadyToExitGameEvent(instanceId);
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

    private void DealCards()
    {
        //currentGameState = GameState.Dealing;
        var cards = Deck.GetShuffledDeck(3);
        var hand = cards.DealCards(5);
        var imageNames = hand.GetImageNames();
    }

    public class Session
    {
        public decimal totalBets;
        public int seatNumber;

        public Session()
        {
            seatNumber = -1;
        }

        public override string ToString()
        {
            return "Your Total Bets: $" + totalBets + "\n" +
                   "Your Seat #: " + (seatNumber + 1);
        }
    }
}