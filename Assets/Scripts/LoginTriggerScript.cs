using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class LoginTriggerScript : MonoBehaviour
{

    static string LOGIN_TEXT = "Press (E) To Login/Register";

    public CanvasGroup loginCanvas;
    public Text instuction;
    bool isInsideTrigger;

    private void OnTriggerEnter(Collider other)
    {
        instuction.text = LOGIN_TEXT;
        isInsideTrigger = true;
    }

    private void OnTriggerExit(Collider other)
    {
        instuction.text = "";
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
                instuction.text = "";
                Cursor.lockState = CursorLockMode.Confined;
                loginCanvas.gameObject.SetActive(true);
            } else if (Input.GetKey(KeyCode.Escape))
            {
                instuction.text = LOGIN_TEXT;
                Cursor.lockState = CursorLockMode.Locked;
                loginCanvas.gameObject.SetActive(false);
            }
        }
    }

}
