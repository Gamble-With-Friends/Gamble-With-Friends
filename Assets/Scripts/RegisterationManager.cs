using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegisterationManager : MonoBehaviour
{

    public CanvasGroup registrationFormCanvasGroup;

    public void functionShowRegisterationForm()
    {
        registrationFormCanvasGroup.gameObject.SetActive(true);
    }

}
