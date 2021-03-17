using UnityEngine;
using Mirror;

public class NetworkManagement : NetworkManager
{

    static readonly Vector2 SPAWN_BEGINNING = new Vector2(-15, 85);
    static readonly Vector2 SPAWN_END = new Vector2(15, 95);

    public override void OnStartServer()
    {
        base.OnStartServer();

        NetworkServer.RegisterHandler<CreateCharacterMessage>(OnCreateCharacter);
    }

    public override void OnStopServer()
    {
        Debug.Log("OnStopServer");
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);

        // you can send the message here, or wherever else you want
        CreateCharacterMessage characterMessage = new CreateCharacterMessage
        {
            name = "Tony",
            playerColor = Color.red
        };

        conn.Send(characterMessage);
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        Debug.Log("OnClientDisconnect");
    }

    void OnCreateCharacter(NetworkConnection conn, CreateCharacterMessage message)
    {
        // playerPrefab is the one assigned in the inspector in Network
        // Manager but you can use different prefabs per race for example
        var gameobject = Instantiate(playerPrefab);

        // Apply data from the message however appropriate for your game
        // Typically Player would be a component you write with syncvars or properties
        var player = gameobject.GetComponent<PlayerMovement>();
        player.name = message.name;

        player.transform.position = new Vector3(Random.Range(SPAWN_BEGINNING.x, SPAWN_END.x), 3, Random.Range(SPAWN_BEGINNING.y, SPAWN_END.y));

        // call this to use this gameobject as the primary controller
        NetworkServer.AddPlayerForConnection(conn, gameobject);
    }
}