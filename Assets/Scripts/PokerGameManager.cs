using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Mirror;
using UnityEngine;

public class PokerGameManager : NetworkBehaviour
{
    private const int MAX_PLAYERS = 1; 
    GameObject chip1;
    GameObject chip5;
    GameObject chip25;
    GameObject chip100;
    public GameObject gameCamera;

    private static List<GameObject> _playerChips;

    private int betAmount;

    [SyncVar] private GameState currentGameState;
    [SyncVar] private string slotToUserIdSerialized;
    
    private Dictionary<int, string> slotToUserId;

    //GameObject variableForPrefab = (GameObject)Resources.Load("Prefabs/FirstPersonPlayer", typeof(GameObject));

    private void Update()
    {
        slotToUserId = (Dictionary<int, string>) JsonUtility.FromJson(slotToUserIdSerialized, typeof(Dictionary<int, string>)) ??
            new Dictionary<int, string>();
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
        if (slotToUserId.Count < MAX_PLAYERS)
        {
            CmdAddPlayer(UserInfo.GetInstance().UserId, slotToUserIdSerialized,null);
        }
        else
        {
            EventManager.FireInstructionChangeEvent("Table is full.");
        }
    }

    private void DealCards()
    {
        currentGameState = GameState.Dealing;
        var cards = Deck.GetShuffledDeck(3);
        var hand = cards.DealCards(5);
        var imageNames = hand.GetImageNames();
    }

    private void OnModifyBetAction(int amount)
    {
        if (currentGameState != GameState.Betting) return;
        betAmount += amount;
        if (betAmount < 0)
        {
            betAmount = 0;
        }

        EventManager.FireInstructionChangeEvent($"You placed ${betAmount}");
    }

    private void OnClick(int instanceId)
    {
        if (instanceId == transform.GetInstanceID())
        {
            EventManager.FirePrepareToGameEvent(instanceId);
        }
    }

    private void OnPrepareToExitGame(int instanceId)
    {
        if (instanceId != transform.GetInstanceID()) return;
        gameCamera.SetActive(false);
        DestroyChips();
        CmdRemovePlayer(UserInfo.GetInstance().UserId, slotToUserIdSerialized);
        EventManager.FireInstructionChangeEvent("");
        EventManager.FireReadyToExitGameEvent(instanceId);
    }

    private void InstantiateChips(int nChip1, int nChip5, int nChip25, int nChip100)
    {
        var chipZ = chip1.transform.localScale.z;
        var positionX = transform.position.x + transform.localScale.x * 0.45f;
        const float positionY = 2.6f;
        var positionZ = transform.position.z + transform.localScale.z * 0.15f;
        var offset = chipZ + chipZ * 0.2f;

        InstantiateStack(chip1, nChip1, positionX, positionY, positionZ);
        positionZ += offset;
        InstantiateStack(chip5, nChip5, positionX, positionY, positionZ);
        positionZ += offset;
        InstantiateStack(chip25, nChip25, positionX, positionY, positionZ);
        positionZ += offset;
        InstantiateStack(chip100, nChip100, positionX, positionY, positionZ);
    }

    private static void InstantiateStack(GameObject chip, int numberOfChips, float posX, float posY, float posZ)
    {
        var chipY = chip.transform.localScale.y;
        chipY += chipY * 0.1f;
        for (var i = 0; i < numberOfChips; i++)
        {
            chip.transform.position = new Vector3(posX, posY, posZ);
            _playerChips.Add(Instantiate(chip, chip.transform));
            posY += chipY;
        }
    }

    private static void DestroyChips()
    {
        if (_playerChips != null)
        {
            foreach (var t in _playerChips)
            {
                Destroy(t);
            }
        }

        _playerChips = new List<GameObject>();
    }

    [Command(requiresAuthority = false)]
    private void CmdAddPlayer(string userId, string slotToUserIdJson, NetworkConnectionToClient sender = null)
    {
        var dict = (Dictionary<int, string>) JsonUtility.FromJson(slotToUserIdSerialized, typeof(Dictionary<int, string>)) ??
                   new Dictionary<int, string>();
        
        for (var i = 0; i < MAX_PLAYERS; i++)
        {
            if (dict.ContainsKey(i)) continue;
            dict.Add(i, userId);
            break;
        }
        
        slotToUserIdSerialized = JsonUtility.ToJson(dict);
    }

    [Command(requiresAuthority = false)]
    private void CmdRemovePlayer(string userId, string slotToUserIdJson, NetworkConnectionToClient sender = null)
    {
        var dict = (Dictionary<int, string>) JsonUtility.FromJson(slotToUserIdJson, typeof(Dictionary<int, string>)) ??
                   new Dictionary<int, string>();
        
        for (var i = 0; i < MAX_PLAYERS; i++)
        {
            if (!dict.ContainsKey(i) || dict[i] != userId) continue;
            dict.Remove(i);
            break;
        }
        
        slotToUserIdSerialized = JsonUtility.ToJson(dict);
    }

    [Command(requiresAuthority = false)]
    private void CmdSetGameState(GameState state)
    {
        currentGameState = state;
    }

    private enum GameState
    {
        NotStarted,
        Betting,
        Dealing
    }
}