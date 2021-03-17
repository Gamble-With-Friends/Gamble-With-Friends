using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class LoginTriggerScript : MonoBehaviour
{
    private const string LOGIN_TEXT = "Press (E) To Login/Register";

    public GameObject loginCanvas; // group containing all groups pertaining to registration and login processes
    public GameObject registerFormGroup; // registration form group
    public GameObject loginFormGroup; // login form group
    public GameObject loginRegistrationGroup; // 'Register' and 'Login' buttons group 

    private bool isInsideTrigger;
    private PlayerMovement collidingPlayerMovement;
    private MouseLook collidingPlayerMouseLook;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.GetComponent<PlayerMovement>().isLocalPlayer) return;
        collidingPlayerMovement = other.gameObject.GetComponent<PlayerMovement>();
        collidingPlayerMouseLook = other.gameObject.transform.Find("Main Camera").GetComponent<MouseLook>();
        EventManager.FireInstructionChangeEvent(LOGIN_TEXT);
        isInsideTrigger = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (collidingPlayerMovement != null && !collidingPlayerMovement.isLocalPlayer) return;
        isInsideTrigger = false;
        EventManager.FireInstructionChangeEvent("");
        ExitRegistrationLogin();
    }

    private void Update()
    {
        if (collidingPlayerMovement != null && !collidingPlayerMovement.isLocalPlayer) return;

        if (!isInsideTrigger) return;

        Cursor.lockState = loginCanvas.activeSelf ? CursorLockMode.Confined : CursorLockMode.Locked;

        if (loginRegistrationGroup.gameObject.activeSelf)
        {
            collidingPlayerMovement.isMovementDisabled = true;
            collidingPlayerMouseLook.disableLookaround = true;
            EventManager.FireInstructionChangeEvent("");
        }

        if (Input.GetKey(KeyCode.E))
        {
            loginCanvas.gameObject.SetActive(true);

            if (!registerFormGroup.activeSelf && !loginFormGroup.activeSelf)
            {
                loginRegistrationGroup.gameObject.SetActive(true);
            }
        }
        else if (Input.GetKey(KeyCode.Escape))
        {
            EventManager.FireInstructionChangeEvent(LOGIN_TEXT);
            ExitRegistrationLogin();
        }
    }

    public void ExitRegistrationLogin()
    {
        if (collidingPlayerMovement != null && !collidingPlayerMovement.isLocalPlayer) return;
        
        Cursor.lockState = CursorLockMode.Locked;
        loginRegistrationGroup.gameObject.SetActive(false);
        loginCanvas.gameObject.SetActive(false);
        registerFormGroup.gameObject.SetActive(false);
        loginFormGroup.gameObject.SetActive(false);
        collidingPlayerMovement.isMovementDisabled = false;
        collidingPlayerMouseLook.disableLookaround = false;
    }
}