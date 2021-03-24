using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendsUITriggerScript : MonoBehaviour
{
    public GameObject FriendsUICanvas;             // group containing all groups pertaining to friends management UI    

    private void OnEnable()
    {
        EventManager.OnKeyDown += OnKeyDown;
        
    }

    private void OnDisable()
    {
        EventManager.OnKeyDown -= OnKeyDown;
        
    }

    private void OnKeyDown(KeyCode key)
    {
        if (key == KeyCode.F)
        {
            Debug.Log("Open Friends UI");
            OpenFriendUI();
        }
    }

    public void OpenFriendUI()
    {
        if (UserInfo.GetInstance().UserId != null)
        {
            if (!FriendsUICanvas.gameObject.activeSelf)
            {
                UserInfo.GetInstance().LockMouse = true;
                UserInfo.GetInstance().LockMovement = true;
                Cursor.lockState = CursorLockMode.Confined;
                FriendsUICanvas.gameObject.SetActive(true);
            }
        }
    }

    public void ExitFriendsUI()
    {
        Cursor.lockState = CursorLockMode.Locked;
        FriendsUICanvas.gameObject.SetActive(false);
        UserInfo.GetInstance().LockMouse = false;
        UserInfo.GetInstance().LockMovement = false;
    }    
}
