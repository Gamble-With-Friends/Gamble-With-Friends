using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CasinoObjectManagerScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {        
        
    }

    // Update is called once per frame
    void Update()
    {
        try
        {
            GameObject variableForPrefab = (GameObject)Resources.Load("Prefabs/FirstPersonPlayer", typeof(GameObject));
            if (variableForPrefab != null)
            {
                Debug.Log("found");
            }

            var myObj = GameObject.Find("FirstPersonPlayer");
            if (myObj != null)
            {
                Debug.Log("found second");
            }

        }
        catch (Exception ex)
        {
            var message = ex.Message;
            var innerMessage = ex.InnerException;
        }
    }
}
