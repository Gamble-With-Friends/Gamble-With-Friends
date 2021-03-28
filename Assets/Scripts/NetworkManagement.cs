using UnityEngine;
using Mirror;

public class NetworkManagement : NetworkManager
{
    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
        
        // Get the user object who disconnected
        Debug.Log("Client Got Disconnected");
    }
}