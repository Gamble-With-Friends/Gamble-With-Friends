using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class PokerGameManager : MonoBehaviour
{
    public GameObject chip1;
    public GameObject chip5;
    public GameObject chip25;
    public GameObject chip100;
    public GameObject gameCamera;
    public GameObject playingCardTest;

    static List<GameObject> playerChips;

    private GameState currentGameState;
    private int betAmount;

    //GameObject variableForPrefab = (GameObject)Resources.Load("Prefabs/FirstPersonPlayer", typeof(GameObject));

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

    private void Start()
    {
        DestroyChips();
        currentGameState = GameState.NotStarted;
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
        if (currentGameState == GameState.NotStarted)
        {
            currentGameState = GameState.Betting;

            InstantiateChips(10, 8, 5, 5);

            EventManager.FireInstructionChangeEvent($"Place your bets...");

            Invoke(nameof(DealCards), 10);
        }
        else
        {
            EventManager.FireInstructionChangeEvent("Game currently in process. Please wait...");
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
            playerChips.Add(Instantiate(chip, chip.transform));
            posY += chipY;
        }
    }

    private static void DestroyChips()
    {
        if (playerChips != null)
        {
            foreach (var t in playerChips)
            {
                Destroy(t);
            }
        }

        playerChips = new List<GameObject>();
    }

    private enum GameState
    {
        NotStarted,
        Betting,
        Dealing
    }
}