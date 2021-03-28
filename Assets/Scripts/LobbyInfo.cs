using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

public class LobbyInfo : NetworkBehaviour
{
    [SyncVar] private string usersStr = "";

    public string serverId;

    [Server]
    private void Awake()
    {
        serverId = FindObjectOfType<NetworkManagement>().GetServerId();
    }

    public static LobbyInfo GetInstance()
    {
        return FindObjectOfType<LobbyInfo>();
    }

    public List<string> GetLoggedInUsers()
    {
        return usersStr.Split('|').ToList();
    }

    public void Login(string username, string password)
    {
        CmdLogin(username, password);
    }

    public void LogoutUser(int connectionId)
    {
        CmdLogout(connectionId);
    }

    [Command(requiresAuthority = false)]
    private void CmdLogin(string username, string password, NetworkConnectionToClient conn = null)
    {
        if (username == null || password == null) return;

        DataManager.UpdateLogoutTime(conn.connectionId, serverId);
        
        var player = DataManager.GetUser(username, password);
        
        if (player == null)
        {
            TargetLoginFailed(conn, username,"Invalid username or password");
        }
        else
        {
            var isPlayerLoggedIn = DataManager.IsPlayerLoggedIn(player.UserId, serverId);
            
            if (!isPlayerLoggedIn)
            {
                DataManager.CreateLoginSession(conn.connectionId, serverId, player.UserId);
                TargetLoginSuccess(conn, player.UserName,player.UserId, player.Coins);
            }
            else
            {
                TargetLoginFailed(conn, username,"Player Already Logged In");
            }
        }
    }

    [Command(requiresAuthority = false)]
    private void CmdLogout(int connectionId)
    {
        DataManager.UpdateLogoutTime(connectionId, serverId);
    }

    [TargetRpc]
    private void TargetLoginSuccess(NetworkConnection conn, string username, string userId, decimal coins)
    {
        EventManager.FireLoginSuccessEvent(username, userId, coins);
    }

    [TargetRpc]
    private void TargetLoginFailed(NetworkConnection conn, string username, string errorMessage)
    {
        EventManager.FireLoginErrorEvent(username, errorMessage);
    }
}