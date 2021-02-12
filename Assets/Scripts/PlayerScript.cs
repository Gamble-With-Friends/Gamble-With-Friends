using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();

        if (isLocalPlayer)
        {
            if(Input.GetKeyDown(KeyCode.X))
            {
                EveryoneServer();
            }

            if(Input.GetKeyDown(KeyCode.Y))
            {
                OnlyYouServer();
            }
        }
    }

    void HandleMovement()
    {
        if (isLocalPlayer)
        {
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");
            Vector3 movement = new Vector3(moveHorizontal, moveVertical, 0);
            transform.position = transform.position + movement;
        }
    }

    [Command]
    void EveryoneServer()
    {
        Debug.Log("EveryoneServer on Server!");
        Everyone();
    }

    [Command]
    void OnlyYouServer()
    {
        Debug.Log("OnlyYouServer on Server!");
        OnlyYou();
    }

    [TargetRpc]
    void OnlyYou()
    {
        Debug.Log("Only you can see this");
    }

    [ClientRpc]
    void Everyone()
    {
        Debug.Log("Everyone can see this");
    }

}
