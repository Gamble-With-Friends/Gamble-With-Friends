using UnityEngine;

public class PokerClient : MonoBehaviour
{
    public const int MaxPlayers = 1;
    public TextMesh informationText;
    public PokerServer server;
    public GameObject gameCamera;
    private int betAmount;

    //GameObject variableForPrefab = (GameObject)Resources.Load("Prefabs/FirstPersonPlayer", typeof(GameObject));

    private void Update()
    {
        informationText.text = "Total Players: " + GetTotalPlayers();
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

        betAmount = 0;
        gameCamera.SetActive(true);
        EventManager.FireStartGameEvent(instanceId);
    }

    private void OnStartGame(int instanceId)
    {
        if (!IsTableFull())
        {
            server.AddPlayer(UserInfo.GetInstance().UserId);
        }
        else
        {
            EventManager.FireInstructionChangeEvent("Table is full.");
        }
    }

    private void OnModifyBetAction(int amount)
    {
        if (server.gameState != GameState.Betting) return;
        betAmount += amount;
        if (betAmount < 0)
        {
            betAmount = 0;
        }

        EventManager.FireInstructionChangeEvent($"You placed ${betAmount}");
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

    private void OnPrepareToExitGame(int instanceId)
    {
        if (instanceId != transform.GetInstanceID()) return;
        gameCamera.SetActive(false);
        server.RemovePlayer(UserInfo.GetInstance().UserId);
        EventManager.FireInstructionChangeEvent("");
        EventManager.FireReadyToExitGameEvent(instanceId);
    }

    private bool IsTableFull()
    {
        return GetTotalPlayers() >= MaxPlayers;
    }

    private int GetTotalPlayers()
    {
        return server.slotToUserId?.Count ?? 0;
    }

    private void DealCards()
    {
        //currentGameState = GameState.Dealing;
        var cards = Deck.GetShuffledDeck(3);
        var hand = cards.DealCards(5);
        var imageNames = hand.GetImageNames();
    }
}