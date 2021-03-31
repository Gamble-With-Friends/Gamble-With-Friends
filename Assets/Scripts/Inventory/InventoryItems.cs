using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItems : MonoBehaviour
{
    public static Dictionary<string, InventoryItem> itemIdToRecord;

    public class InventoryItem
    {
        public string ItemId { get; set; }
        public bool Equipped { get; set; }
        public DateTime PurchaseDate { get; set; }
        public int Payouts { get; set;}
    }

    public static Dictionary<string, InventoryItem> GetInventoryItems()
    {
        if (itemIdToRecord == null)
        {
            DataManager.GetInventoryItems(UserInfo.GetInstance().UserId);
        }

        return itemIdToRecord;
    }

    public static void UpdateItems(string userId)
    {
        DataManager.GetInventoryItems(userId);
    }
}
