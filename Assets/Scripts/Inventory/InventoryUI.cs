﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public Text CoinsValue;
    public GameObject InventoryCanvas;

    private void OnEnable()
    {
        EventManager.OnKeyDown += OnKeyDown;
    }

    private void OnDisable()
    {
        EventManager.OnKeyDown -= OnKeyDown;
    }


    private void OnKeyDown(KeyCode key)
    {
        if (UserInfo.GetInstance().UserId != null)
        {
            if (key == KeyCode.I)
            {
                if (InventoryCanvas.gameObject.activeSelf)
                {
                    ExitInventory();
                }
                else
                {
                    Cursor.lockState = CursorLockMode.Confined;
                    InventoryCanvas.gameObject.SetActive(true);
                    UserInfo.GetInstance().LockMouse = true;
                    UserInfo.GetInstance().LockMovement = true;
                }
            }
        }
    }

    public void ExitInventory()
    {
        Cursor.lockState = CursorLockMode.Locked;
        InventoryCanvas.gameObject.SetActive(false);
        UserInfo.GetInstance().LockMouse = false;
        UserInfo.GetInstance().LockMovement = false;
    }

    public decimal GetCoinValue()
    {
        return UserInfo.GetInstance().TotalCoins;
    }

    // Update is called once per frame
    void Update()
    {
    }
}
