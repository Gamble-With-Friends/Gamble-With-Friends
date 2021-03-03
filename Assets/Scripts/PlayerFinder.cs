using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFinder : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        GameObject player = FindObjectOfType<PlayerMovement>().gameObject;

        if(player != null)
        {
            Debug.Log("Player is not null");
        } else
        {
            Debug.Log("Player is null");
        }

    }
}
