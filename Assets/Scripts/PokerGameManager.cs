﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokerGameManager : MonoBehaviour
{
    public GameObject chip1;
    public GameObject chip5;
    public GameObject chip25;
    public GameObject chip100;
    public GameObject gameCamera;

    static List<GameObject> playerChips;
    int betAmount;

    void OnEnable()
    {
        EventManager.OnClick += OnClick;
        EventManager.OnReadyToGame += OnReadyToGame;
        EventManager.OnPrepareToExitGame += OnPrepareToExitGame;
        EventManager.OnModifyBetAction += OnModifyBetAction;
    }

    void OnDisable()
    {
        EventManager.OnClick -= OnClick;
        EventManager.OnReadyToGame -= OnReadyToGame;
        EventManager.OnPrepareToExitGame -= OnPrepareToExitGame;
        EventManager.OnModifyBetAction -= OnModifyBetAction;
    }

    private void Start()
    {
        DestoryChips();
    }

    void OnReadyToGame(int instanceId)
    {
        if (instanceId == transform.GetInstanceID())
        {
            betAmount = 0;
            gameCamera.SetActive(true);
            InstatiateChips(10, 8, 5, 5);
        }
    }

    void OnModifyBetAction(int amount)
    {
        betAmount += amount;
        Debug.Log("Current Amount: " + betAmount);
    }

    void OnClick(int instanceId)
    {
        if (instanceId == transform.GetInstanceID())
        {
            EventManager.FirePrepareToGameEvent(instanceId);
        }
    }

    void OnPrepareToExitGame(int instanceId)
    {
        if (instanceId == transform.GetInstanceID())
        {
            gameCamera.SetActive(false);
            DestoryChips();
            EventManager.FireReadyToExitGameEvent(instanceId);
        }
    }

    void InstatiateChips(int nChip1, int nChip5, int nChip25, int nChip100)
    {
        float chipZ = chip1.transform.localScale.z;
        float positionX = transform.position.x + transform.localScale.x * 0.45f;
        float positionY = 2.6f;
        float positionZ = transform.position.z + transform.localScale.z * 0.15f;
        float offset = chipZ + chipZ * 0.2f;

        InstatiateStack(chip1, nChip1, positionX, positionY, positionZ);
        positionZ += offset;
        InstatiateStack(chip5, nChip5, positionX, positionY, positionZ);
        positionZ += offset;
        InstatiateStack(chip25, nChip25, positionX, positionY, positionZ);
        positionZ += offset;
        InstatiateStack(chip100, nChip100, positionX, positionY, positionZ);
    }

    void InstatiateStack(GameObject chip, int numberOfChips, float posX, float posY, float posZ)
    {
        float chipY = chip.transform.localScale.y;
        chipY += chipY * 0.1f;
        for (int i = 0; i < numberOfChips; i++)
        {
            chip.transform.position = new Vector3(posX, posY, posZ);
            playerChips.Add(Instantiate(chip, chip.transform));
            posY += chipY;
        }
    }

    void DestoryChips()
    {
        if (playerChips != null)
        {
            for (int i = 0; i < playerChips.Count; i++)
            {
                Destroy(playerChips[i]);
            }
        }

        playerChips = new List<GameObject>();
    }
}