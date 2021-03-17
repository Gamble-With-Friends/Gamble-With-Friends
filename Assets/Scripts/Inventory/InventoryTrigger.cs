using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryTrigger : MonoBehaviour
{

    public GameObject InventoryCanvas;             // group containing all groups pertaining to registration and login processes

    bool isInsideTrigger;

    private PlayerMovement collidingPlayerMovement;
    private MouseLook collidingPlayerMouseLook;

    private void OnTriggerEnter(Collider other)
    {
        isInsideTrigger = true;
        collidingPlayerMovement = other.gameObject.GetComponent<PlayerMovement>();
        collidingPlayerMouseLook = other.gameObject.transform.Find("Main Camera").GetComponent<MouseLook>();
    }

    private void OnTriggerExit(Collider other)
    {
        isInsideTrigger = false;
        ExitInventory();
    }

    void Update()
    {
        if (isInsideTrigger)
        {
            if (InventoryCanvas.gameObject.activeSelf)
            {
                collidingPlayerMovement.isMovementDisabled = true;
                collidingPlayerMouseLook.disableLookaround = true;
            }

            if (Input.GetKeyDown(KeyCode.I))
            {
                if (InventoryCanvas.gameObject.activeSelf)
                {
                    ExitInventory();
                }
                else
                {
                    Cursor.lockState = CursorLockMode.Confined;
                    InventoryCanvas.gameObject.SetActive(true);
                }

            }
            else if (Input.GetKey(KeyCode.Escape))
            {
                ExitInventory();
            }
        }
    }

    public void ExitInventory()
    {
        Cursor.lockState = CursorLockMode.Locked;
        InventoryCanvas.gameObject.SetActive(false);
        collidingPlayerMovement.isMovementDisabled = false;
        collidingPlayerMouseLook.disableLookaround = false;
    }
}
