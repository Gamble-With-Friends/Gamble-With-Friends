using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class LoginTriggerScript : MonoBehaviour
{

    static string LOGIN_TEXT = "Press (E) To Login/Register";

    public CanvasGroup loginCanvas;
    public Text instruction;
    bool isInsideTrigger;

    private void OnTriggerEnter(Collider other)
    {
        instruction.text = LOGIN_TEXT;
        isInsideTrigger = true;
    }

    private void OnTriggerExit(Collider other)
    {
        instruction.text = "";
        isInsideTrigger = false;
        Cursor.lockState = CursorLockMode.Locked;
        loginCanvas.gameObject.SetActive(false);
    }

    void Update()
    {
        if(isInsideTrigger)
        {
            if(Input.GetKey(KeyCode.E))
            {
                instruction.text = "";
                Cursor.lockState = CursorLockMode.Confined;
                loginCanvas.gameObject.SetActive(true);
            } else if (Input.GetKey(KeyCode.Escape))
            {
                instruction.text = LOGIN_TEXT;
                Cursor.lockState = CursorLockMode.Locked;
                loginCanvas.gameObject.SetActive(false);
            }
        }
    }

}
