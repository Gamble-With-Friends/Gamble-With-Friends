using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginChecker : MonoBehaviour
{
    public BoxCollider collider;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.GetComponent<PlayerMovement>().isLocalPlayer) return;
        
        collider.enabled = UserInfo.GetInstance().UserId == null;

        if (collider.enabled)
        {
            EventManager.FireInstructionChangeEvent("To enter the Casino, you first need to log in at the reception desk.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.GetComponent<PlayerMovement>().isLocalPlayer) return;
        EventManager.FireInstructionChangeEvent("");
    }
}
