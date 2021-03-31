using System;
using System.Collections;
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
        EventManager.OnLoginSuccess += OnLoginSuccess;
    }

    private void OnDisable()
    {
        EventManager.OnKeyDown -= OnKeyDown;
        EventManager.OnLoginSuccess -= OnLoginSuccess;
    }


    private void OnKeyDown(KeyCode key)
    {
        if (UserInfo.GetInstance().UserId == null) return;
        if (key != KeyCode.I) return;
        
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

    private void OnLoginSuccess()
    {
        InventoryItems.UpdateItems();
        PayInvestment();
    }

    private void PayInvestment()
    {
        decimal amount = 0;

        var investments = new string[] { "CryptominingRig", "VendingMachine", "HotdogStand", "CarWash", "ParkingLot", "ConvenienceStore", "McDonalds", "Casino" };
        InventoryItems.UpdateItems();

        foreach (var invest in investments)
        {
            var itemId = GameItems.GetItems()[invest].ItemId;

            if (!InventoryItems.GetInventoryItems().ContainsKey(itemId)) continue;
            var difference = (int)(DateTime.Now - InventoryItems.GetInventoryItems()[itemId].PurchaseDate).TotalDays;
            if (difference <= InventoryItems.GetInventoryItems()[itemId].Payouts) continue;
            var payout = InventoryItems.GetInventoryItems()[itemId].Payouts;
            amount += GameItems.GetItems()[invest].IncomeAmount * Math.Abs((Convert.ToDecimal((int)(InventoryItems.GetInventoryItems()[itemId].PurchaseDate - DateTime.Now).TotalDays)-payout));

            payout += difference;
            DataManager.UpdateInvestmentPayout(UserInfo.GetInstance().UserId, itemId, payout);
        }
        EventManager.FireChangeCoinValue(amount);
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
