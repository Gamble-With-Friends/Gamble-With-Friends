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
                    if(InventoryItems.GetInventoryItems()[itemId].Equipped)
                    {
                        switch (skinname)
                        {
                            case "Hat":
                                hat.SetActive(true);
                                break;
                            case "T-Shirt":
                                tshirt.SetActive(true);
                                break;
                            case "Sweater":
                                sweater.SetActive(true);
                                break;
                            case "Shoes":
                                shoes.SetActive(true);
                                break;
                            case "Jeans":
                                jeans.SetActive(true);
                                break;
                            case "Watch":
                                watch.SetActive(true);
                                break;
                            case "Headphones":
                                headphones.SetActive(true);
                                break;
                            case "Glasses":
                                glasses.SetActive(true);
                                break;
                        }
                    }
                    else
                    {
                        switch (skinname)
                        {
                            case "Hat":
                                hat.SetActive(false);
                                break;
                            case "T-Shirt":
                                tshirt.SetActive(false);
                                break;
                            case "Sweater":
                                sweater.SetActive(false);
                                break;
                            case "Shoes":
                                shoes.SetActive(false);
                                break;
                            case "Jeans":
                                jeans.SetActive(false);
                                break;
                            case "Watch":
                                watch.SetActive(false);
                                break;
                            case "Headphones":
                                headphones.SetActive(false);
                                break;
                            case "Glasses":
                                glasses.SetActive(false);
                                break;
                        }
                    }
                }
            }
        }
    }
}
