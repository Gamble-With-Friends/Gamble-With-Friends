using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class AutoHostClient : MonoBehaviour
{
    public bool forceOffline;

    [SerializeField] NetworkManager networkManager;

    void Start()
    {
       if (!Application.isBatchMode)
        {
            if(forceOffline)
            {
                Debug.Log("Client Build");
                networkManager.networkAddress = "localhost";
                networkManager.StopHost();
                networkManager.StartServer();
                networkManager.StartClient();
            } else
            {
                networkManager.StartClient();
            }
            
        } else
        {
            Debug.Log("Server Build");
        }
    }

    public void JoinLocal()
    {
        networkManager.networkAddress = "localhost";
        networkManager.StopClient();
        networkManager.StartClient();
    }

    public void HostLocalServer()
    {
        networkManager.networkAddress = "localhost";
        networkManager.StartServer();
    }
}
