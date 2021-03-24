using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameItems : MonoBehaviour
{
    public static Dictionary<string, Item> itemNameToRecord;

    public class Item
    {
        public string ItemId { get; set; }
        public string ItemTitle { get; set; }
        public int ItemType { get; set; }
        public decimal CoinValue { get; set; }
        public decimal IncomeAmount { get; set; }
    }

    public static Dictionary<string, Item> GetItems()
    {
        if (itemNameToRecord == null)
        {
            DataManager.GetItems();
        }

        return itemNameToRecord;
    }
}
