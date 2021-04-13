using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInfo
{
    public string UserId { get; set; }
    public string DisplayName { get; set; }
    public decimal DailyIncome { get; set; }
    public decimal TotalCoins { get; set; }
    public int CurrentGameId { get; set; }
    public bool LockMovement { get; set; }
    public bool LockMouse { get; set; }

    private static UserInfo _instance;
    
    public static UserInfo GetInstance()
    {
        return _instance ?? (_instance = new UserInfo{CurrentGameId = -1});
    }

    public bool IsInGame()
    {
        return CurrentGameId != -1;
    }
}
