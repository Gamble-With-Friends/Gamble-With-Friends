using UnityEngine;
using Mirror;
using UnityEditor;

public class NetworkManagement : NetworkManager
{
    private string serverId;
    
    [Server]
    public override void OnServerConnect(NetworkConnection conn)
    {
        base.OnServerConnect(conn);
        Debug.Log("OnServerConnect ConnectionId: " + conn.connectionId);
        Debug.Log("OnServerConnect identity: " + conn.identity);
    }

    [Server]
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
        Debug.Log("OnServerDisconnect ConnectionId: " + conn.connectionId);
        Debug.Log("OnServerDisconnect identity: " + conn.identity);
        
        LobbyInfo.GetInstance().LogoutUser(conn.connectionId);
    }

    [Server]
    public override void OnStartServer()
    {
        base.OnStartServer();
        serverId = GUID.Generate().ToString();
        Debug.Log("OnStartServer ServerID: " + serverId);
    }

    [Server]
    public string GetServerId()
    {
        return serverId;
    }
}