using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class LoginTriggerScript : MonoBehaviour
{

    static string LOGIN_TEXT = "Press (E) To Login/Register";

    public CanvasGroup loginCanvas;
    public GameObject registerFormGroup;
    public GameObject loginFormGroup;
    public GameObject loginRegistrationGroup;

    public Text instruction;
    bool isInsideTrigger;

    private PlayerMovement collidingPlayerMovement;
    private MouseLook collidingPlayerMouseLook;

    private void OnTriggerEnter(Collider other)
    {
        instruction.text = LOGIN_TEXT;
        isInsideTrigger = true;
        collidingPlayerMovement = other.gameObject.GetComponent<PlayerMovement>();
        collidingPlayerMouseLook = other.gameObject.transform.Find("Main Camera").GetComponent<MouseLook>();
    }

    private void OnTriggerExit(Collider other)
    {
        isInsideTrigger = false;
        instruction.text = "";
        ExitRegistrationLogin();
    }

    void Update()
    {
        if(isInsideTrigger)
        {
            Cursor.lockState = loginRegistrationGroup.activeSelf ? CursorLockMode.Confined : CursorLockMode.Locked;

            if (loginRegistrationGroup.gameObject.activeSelf)
            {
                collidingPlayerMovement.isMovementDisabled = true;
                collidingPlayerMouseLook.disableLookaround = true;
                instruction.text = "";
            }

            if (Input.GetKey(KeyCode.E))
            {
                loginRegistrationGroup.gameObject.SetActive(true);

                if (!registerFormGroup.activeSelf && !loginFormGroup.activeSelf)
                {
                    loginCanvas.gameObject.SetActive(true);
                }
            }
            else if (Input.GetKey(KeyCode.Escape))
            {
                instruction.text = LOGIN_TEXT;
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
