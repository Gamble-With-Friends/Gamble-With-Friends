using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outfit
{
    private string[] skins = {"Hat", "T-Shirt", "Sweater", "Shoes", "Jeans", "Watch", "Headphones", "Glasses"};

    public bool hat { get; private set; }
    public bool tshirt { get; private set; }
    public bool shoes { get; private set; }
    public bool jeans { get; private set; }
    public bool watch { get; private set; }
    public bool sweater { get; private set; }
    public bool headphones { get; private set; }
    public bool glasses { get; private set; }

    public Outfit(string userId)
    {
        hat = false;
        tshirt = false;
        shoes = false;
        jeans = false;
        watch = false;
        sweater = false;
        headphones = false;
        glasses = false;

        var itemIdToInventoryItem = InventoryItems.GetInventoryItems(userId);

        foreach (var skinname in skins)
        {
            var itemId = GameItems.GetItems()[skinname].ItemId;

            if (!itemIdToInventoryItem.ContainsKey(itemId)) continue;

            if (itemIdToInventoryItem[itemId].Equipped)
            {
                SetTrue(skinname);
            }
        }
    }

    private void SetTrue(string skinname)
    {
        switch (skinname)
        {
            case "Hat":
                hat = true;
                break;
            case "T-Shirt":
                tshirt = true;
                break;
            case "Sweater":
                sweater = true;
                break;
            case "Shoes":
                shoes = true;
                break;
            case "Jeans":
                jeans = true;
                break;
            case "Watch":
                watch = true;
                break;
            case "Headphones":
                headphones = true;
                break;
            case "Glasses":
                glasses = true;
                break;
        }
    }
}