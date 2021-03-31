using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryPlayerEquip : MonoBehaviour
{
    public GameObject hat;
    public GameObject tshirt;
    public GameObject shoes;
    public GameObject jeans;
    public GameObject watch;
    public GameObject sweater;
    public GameObject headphones;
    public GameObject glasses;


    string[] skins = new string[] { "Hat", "T-Shirt", "Sweater", "Shoes", "Jeans", "Watch", "Headphones", "Glasses" };

    public void OnEnable()
    {
        EventManager.OnOutfitChange += OnOutfitChange;
    }

    public void OnDisable()
    {
        EventManager.OnOutfitChange += OnOutfitChange;
    }

    private void ChangeOutfit (string skinname, bool equip)
    {
        switch (skinname)
        {
            case "Hat":
                hat.SetActive(equip);
                break;
            case "T-Shirt":
                tshirt.SetActive(equip);
                break;
            case "Sweater":
                sweater.SetActive(equip);
                break;
            case "Shoes":
                shoes.SetActive(equip);
                break;
            case "Jeans":
                jeans.SetActive(equip);
                break;
            case "Watch":
                watch.SetActive(equip);
                break;
            case "Headphones":
                headphones.SetActive(equip);
                break;
            case "Glasses":
                glasses.SetActive(equip);
                break;
        }
    }

    private void OnOutfitChange()
    {
        if(UserInfo.GetInstance().UserId != null)
        {
            InventoryItems.UpdateItems();

            foreach (string skinname in skins)
            {
                var itemId = GameItems.GetItems()[skinname].ItemId;

                if (InventoryItems.GetInventoryItems().ContainsKey(itemId))
                {
                    if (InventoryItems.GetInventoryItems()[itemId].Equipped)
                    {
                        ChangeOutfit(skinname, true);
                    }
                    else
                    {
                        ChangeOutfit(skinname, false);
                    }
                }
                else
                {
                    ChangeOutfit(skinname, false);
                }
            }
        }
    }
}
