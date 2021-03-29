using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginChecker : MonoBehaviour
{
    public BoxCollider collider;
    public GameObject doorLeft;
    public GameObject doorRight;

    private void OnTriggerEnter(Collider other)
    {
        collider.enabled = UserInfo.GetInstance().UserId == null;
    }
}
