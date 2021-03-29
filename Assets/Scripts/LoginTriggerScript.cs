using System;
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
    public RegisterLoginScript registerLoginScript;

    private bool isInsideTrigger;
    private PlayerMovement collidingPlayerMovement;


    private void OnEnable()
    {
        EventManager.OnKeyDown += OnKeyDown;
    }

    private void OnDisable()
    {
        EventManager.OnKeyDown -= OnKeyDown;
    }

    private void OnKeyDown(KeyCode keyCode)
    {
        if (!isInsideTrigger) return;
        
        if (Input.GetKey(KeyCode.E))
        {
            loginCanvas.gameObject.SetActive(true);
            loginFormGroup.gameObject.SetActive(true);
            Cursor.lockState = CursorLockMode.Confined;
            UserInfo.GetInstance().LockMovement = true;
            UserInfo.GetInstance().LockMouse = true;
            EventManager.FireInstructionChangeEvent("");
            registerLoginScript.OpenLoginForm();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.GetComponent<PlayerMovement>().isLocalPlayer) return;
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

    public void ExitRegistrationLogin()
    {
        Cursor.lockState = CursorLockMode.Locked;
        loginCanvas.gameObject.SetActive(false);
        registerFormGroup.gameObject.SetActive(false);
        loginFormGroup.gameObject.SetActive(false);
        UserInfo.GetInstance().LockMovement = false;
        UserInfo.GetInstance().LockMouse = false;
    }
}