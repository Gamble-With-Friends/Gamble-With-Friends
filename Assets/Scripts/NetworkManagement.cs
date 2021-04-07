﻿using UnityEngine;
using Mirror;
using UnityEditor;

public class NetworkManagement : NetworkManager
{
    private string serverId;
    
    [Server]
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
        LobbyInfo.GetInstance().LogoutUser(conn.connectionId);
    }

    [Server]
    public override void OnStartServer()
    {
        base.OnStartServer();
        serverId = GUID.Generate().ToString();
    }

    [Server]
    public string GetServerId()
    {
        return serverId;
    }
}