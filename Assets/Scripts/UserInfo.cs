using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInfo
{
    public string UserId { get; set; }
    public string DisplayName { get; set; }
    public decimal TotalCoins { get; set; }
    public bool LockMovement { get; set; }
    public bool LockMouse { get; set; }

    private static UserInfo _instance;
    
    public static UserInfo GetInstance()
    {
        return _instance ?? (_instance = new UserInfo());
    }
}
