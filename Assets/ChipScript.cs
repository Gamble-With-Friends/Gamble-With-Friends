using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChipScript : MonoBehaviour
{

    private void OnEnable()
    {
        EventManager.onClick += ClickHandler;
    }

    private void OnDisable()
    {
        EventManager.onClick -= ClickHandler;
    }

    void ClickHandler(string gameObjectName)
    {
        Debug.Log("object name: " + gameObjectName + " my name: " + name);

        if (name == gameObjectName)
        {

        }
    }
}
