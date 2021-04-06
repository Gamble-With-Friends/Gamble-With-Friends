using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryButton : MonoBehaviour
{
    public Button sellButton;
    public Button equipButton;
    public Button unequipButton;
    public Image icon;
    public GameObject description;
    public Text descriptionText;
    public string itemName;
    public GameObject errorWindow;
    public Text errorText;
    private bool displayErrorMessage = false;
    private int timerCounter = 0;

    public void OnItemClick()
    {
        if (sellButton.interactable != true)
        {
            if (itemName != null)
            {
                if (UserInfo.GetInstance().TotalCoins > GameItems.GetItems()[itemName].CoinValue)
                {
                    Color shade = icon.color;
                    shade.a = 1f;
                    icon.color = shade;
                    DataManager.Buyitem(UserInfo.GetInstance().UserId, itemName);
                    EventManager.FireChangeCoinValue(-GameItems.GetItems()[itemName].CoinValue);
                    sellButton.interactable = true;
                    if (GameItems.GetItems()[itemName].ItemType != 2)
                    {
                        equipButton.interactable = true;
                    }
                }
                else
                {
                    errorText.text = "Insufficient funds";
                    displayErrorMessage = true;
                    errorWindow.SetActive(true);
                }
            }
        }
    }

    public void OnSellClick()
    {
        decimal discountFactor = 0.8M;
        if (sellButton.interactable == true)
        {
            Color shade = icon.color;
            shade.a = 0.3f;
            icon.color = shade;

            if (itemName != null)
            {
                DataManager.SellItem(UserInfo.GetInstance().UserId, GameItems.GetItems()[itemName].ItemId);
                EventManager.FireChangeCoinValue(discountFactor * GameItems.GetItems()[itemName].CoinValue);
                EventManager.FireRequestOutfitChange(UserInfo.GetInstance().UserId);
            }
            equipButton.interactable = false;
            sellButton.interactable = false;
            unequipButton.interactable = false;
        }
    }

    public void OnEquipClick()
    {
        if (equipButton.interactable == true)
        {

            if (itemName != null)
            {
                DataManager.EquiptItem(UserInfo.GetInstance().UserId, itemName);
                EventManager.FireRequestOutfitChange(UserInfo.GetInstance().UserId);
            }

            equipButton.interactable = false;
            unequipButton.interactable = true;
        }
    }

    public void OnUnequipClick()
    {
        if (unequipButton.interactable == true)
        {
            if (itemName != null)
            {
                DataManager.UnequiptItem(UserInfo.GetInstance().UserId, itemName);
                EventManager.FireRequestOutfitChange(UserInfo.GetInstance().UserId);
            }
            unequipButton.interactable = false;
            equipButton.interactable = true;
        }
    }

    public decimal GetCoinValue()
    {
        return UserInfo.GetInstance().TotalCoins;
    }


    public void OnPointerEnter()
    {
        description.SetActive(true);
        descriptionText.text = itemName + "\n$" + GameItems.itemNameToRecord[itemName].CoinValue + 
            "\nIncome Per day: "+ GameItems.itemNameToRecord[itemName].IncomeAmount;
    }

    public void OnPointerExit()
    {
        description.SetActive(false);
    }
    
    void OnEnable()
    {
        InventoryItems.UpdateItems();
 
        string itemId = null;

        if (GameItems.GetItems().ContainsKey(itemName))
        {
            itemId = GameItems.GetItems()[itemName].ItemId;
        }

        if (itemId != null)
        {
            bool equip = false;

            if (InventoryItems.GetInventoryItems().ContainsKey(itemId))
            {
                equip = InventoryItems.GetInventoryItems()[itemId].Equipped;
                Color shade = icon.color;
                shade.a = 1f;
                icon.color = shade;
                sellButton.interactable = true;
                if (GameItems.GetItems()[itemName].ItemType != 2)
                {
                    equipButton.interactable = !equip;
                    unequipButton.interactable = equip;
                }
            }
            else
            {
                Color shade = icon.color;
                shade.a = 0.3f;
                icon.color = shade;
                sellButton.interactable = false;
                equipButton.interactable = false;
                unequipButton.interactable = false;
            }
        }

        EventManager.FireRequestOutfitChange(UserInfo.GetInstance().UserId);
    }

        // Update is called once per frame
    void Update()
    {
        if (displayErrorMessage)
        {
            timerCounter++;
            if (timerCounter >= 150)
            {
                displayErrorMessage = false;
                errorWindow.SetActive(false);
                timerCounter = 0;
            }
        }
    }

}
