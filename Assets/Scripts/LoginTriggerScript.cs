using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class LoginTriggerScript : MonoBehaviour
{

    static string LOGIN_TEXT = "Press (E) To Login/Register";

    public GameObject loginCanvas;             // group containing all groups pertaining to registration and login processes
    public GameObject registerFormGroup;        // registration form group
    public GameObject loginFormGroup;           // login form group
    public GameObject loginRegistrationGroup;   // 'Register' and 'Login' buttons group 

    bool isInsideTrigger;

    private PlayerMovement collidingPlayerMovement;
    private MouseLook collidingPlayerMouseLook;

    private void OnTriggerEnter(Collider other)
    {
        EventManager.FireInstructionChangeEvent(LOGIN_TEXT);
        isInsideTrigger = true;
        collidingPlayerMovement = other.gameObject.GetComponent<PlayerMovement>();
        collidingPlayerMouseLook = other.gameObject.transform.Find("Main Camera").GetComponent<MouseLook>();
    }

    private void OnTriggerExit(Collider other)
    {
        isInsideTrigger = false;
        EventManager.FireInstructionChangeEvent("");
        ExitRegistrationLogin();
    }

    void Update()
    {
        if(isInsideTrigger)
        {
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
    }

    public void ExitRegistrationLogin()
    {
        Cursor.lockState = CursorLockMode.Locked;
        loginRegistrationGroup.gameObject.SetActive(false);
        loginCanvas.gameObject.SetActive(false);
        registerFormGroup.gameObject.SetActive(false);
        loginFormGroup.gameObject.SetActive(false);
        collidingPlayerMovement.isMovementDisabled = false;
        collidingPlayerMouseLook.disableLookaround = false;
    }

}
