using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChipScript : MonoBehaviour
{
    void OnEnable()
    {
        EventManager.OnClick += OnClick;
    }

    private void OnDisable()
    {
        EventManager.OnClick -= OnClick;
    }

    void OnClick(int instanceId)
    {
        if (instanceId == transform.GetInstanceID())
        {
            if(Input.GetKeyUp(KeyCode.Mouse0) || Input.GetKeyUp(KeyCode.Mouse1))
            {
                int amount = 0;
                if (name.Contains("100"))
                {
                    amount = 100;
                }
                else if (name.Contains("25"))
                {
                    amount = 25;
                }
                else if (name.Contains("5"))
                {
                    amount = 5;
                }
                else if (name.Contains("1"))
                {
                    amount = 1;
                }

                if(Input.GetKeyUp(KeyCode.Mouse1))
                {
                    amount *= -1;
                }

                EventManager.FireOnModifyBetEvent(amount);
            }
        }
    }
}