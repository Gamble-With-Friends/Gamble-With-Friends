using System.Collections;
using System.Collections.Generic;
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

    public void OnItemClick()
    {
        if (sellButton.interactable != true)
        {
            Color shade = icon.color;
            shade.a = 1f;
            icon.color = shade;
            if (itemName != null)
            {
                DataManager.Buyitem(UserInfo.GetInstance().UserId, itemName);
                EventManager.FireChangeCoinValue(-GameItems.GetItems()[itemName].CoinValue);
            }

            sellButton.interactable = true;
            equipButton.interactable = true;
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
                EventManager.FireOutfitChange();
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
                EventManager.FireOutfitChange();
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
                EventManager.FireOutfitChange();
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
        descriptionText.text = itemName;
    }

    public void OnPointerExit()
    {
        description.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
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
                equipButton.interactable = !equip;
                unequipButton.interactable = equip;
                EventManager.FireOutfitChange();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
