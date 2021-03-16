using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModelScript
{
    public string PlayerId { get; set; }
    public string UserName { get; set; }
    public decimal Coins { get; set; }

    public PlayerModelScript(string id, string displayName, decimal money) {
        PlayerId = id;
        UserName = displayName;
        Coins = money;
    }
}
