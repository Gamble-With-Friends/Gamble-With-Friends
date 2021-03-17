using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public Text CoinsValue;

    public decimal GetCoinValue()
    {
        return UserInfo.GetInstance().TotalCoins;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (UserInfo.GetInstance().UserId != null)
        {
            CoinsValue.text = GetCoinValue().ToString();
        }
    }
}
