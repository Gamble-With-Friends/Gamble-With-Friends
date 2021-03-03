using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class AutoHostClient : MonoBehaviour
{

    NetworkManager networkManager;

    void Start()
    {
        networkManager = FindObjectOfType<NetworkManagement>();

       if (!Application.isBatchMode)
        {
            if(!Application.isEditor)
            {
                networkManager.StartClient();
            }
        }
    }

    public void JoinLocal()
    {
        networkManager.networkAddress = "localhost";
        networkManager.StartClient();
    }

    public void HostLocalServer()
    {
        networkManager.networkAddress = "localhost";
        networkManager.StartServer();
    }
}
