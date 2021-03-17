using UnityEngine;
using Mirror;

public class NetworkManagement : NetworkManager
{
    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        Debug.Log(conn.address + " connected!");
    }
}