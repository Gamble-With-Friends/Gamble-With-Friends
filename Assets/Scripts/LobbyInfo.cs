using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

public class LobbyInfo : NetworkBehaviour
{
    [SyncVar]
    private string usersStr = "";

    public static LobbyInfo GetInstance()
    {
        return FindObjectOfType<LobbyInfo>();
    }

    public List<string> GetLoggedInUsers()
    {
        return usersStr.Split('|').ToList();
    }

    public void AddUser(string userId)
    {
        CmdAddUser(userId);
    }

    public void RemoveUser(string userId)
    {
        CmdRemoveUser(userId);
    }

    [Command(requiresAuthority = false)]
    private void CmdAddUser(string userId)
    {
        if (userId == null) return;

        var users = usersStr.Split('|').ToList();

        userId = userId.ToLower();

        if (!users.Contains(userId))
        {
            users.Add(userId);
        }

        usersStr = string.Join("|",users);
    }

    [Command(requiresAuthority = false)]
    private void CmdRemoveUser(string userId)
    {
        var users = usersStr.Split('|').ToList();
        
        if (userId == null) return;

        userId = userId.ToLower();

        if (users.Contains(userId))
        {
            users.Remove(userId);
        }
        
        usersStr = string.Join("|",users);
    }
}