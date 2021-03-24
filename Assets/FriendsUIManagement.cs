using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendsUIManagement : MonoBehaviour
{
    public GameObject FriendsUICanvas;
    public GameObject SendCoinsCanvas;

    private void OnEnable()
    {
        EventManager.OnFriendRawActionClick += OnFriendRawActionClick;
        EventManager.OnKeyDown += OnKeyDown;
    }

    private void OnDisable()
    {
        EventManager.OnFriendRawActionClick -= OnFriendRawActionClick;
        EventManager.OnKeyDown -= OnKeyDown;
    }

    #region Friends UI
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

    public void CloseFriendsUI()
    {
        Cursor.lockState = CursorLockMode.Locked;
        FriendsUICanvas.gameObject.SetActive(false);
        UserInfo.GetInstance().LockMouse = false;
        UserInfo.GetInstance().LockMovement = false;
    }
    #endregion

    #region Send Coins UI
    private void OnFriendRawActionClick(FriendRawAction action, string displayName)
    {
        if (action == FriendRawAction.SendCoins)
        {
            Debug.Log("Open Send Coins");
            OpenSendCoinsUI(displayName);
        }
    }

    public void OpenSendCoinsUI(string displayName)
    {
        Cursor.lockState = CursorLockMode.Confined;
        SendCoinsCanvas.SetActive(true);
        var sendCoinsTitle = GameObject.Find("SendCoinsRawImage/WindowTitle");
        sendCoinsTitle.GetComponent<Text>().text = string.Format("Send coins to {0}", displayName);
        GameObject.Find("SendCoinsInput/InputField").GetComponent<InputField>().text = "";
        GameObject.Find("SendCoinsRawImage/ErrorMessage").GetComponent<Text>().text = "";
    }

    public void SendCoinsToFriend()
    {
        var localUserId = UserInfo.GetInstance().UserId;

        var friendDisplayName = GameObject.Find("SendCoinsRawImage/WindowTitle").GetComponent<Text>().text.Substring(14);

        var coinsString = GameObject.Find("SendCoinsInput/InputField").GetComponent<InputField>().text;

        var coinsValue = 0;
        bool parsed = Int32.TryParse(coinsString, out coinsValue);

        // what do we do if it wasn't successfully parsed?
        if (parsed)
        {
            decimal coinsInWallet = 0M;

            if (!DataManager.GetUserFunds(localUserId, out coinsInWallet))
            {
                GameObject.Find("SendCoinsRawImage/ErrorMessage").GetComponent<Text>().text = "Unexpected error occurred";
            }            
            else if (coinsInWallet < coinsValue)
            {
                GameObject.Find("SendCoinsRawImage/ErrorMessage").GetComponent<Text>().text = "You don't have enough funds!";                
            }
            else
            {                
                DataManager.AddCoinsByDisplayName(friendDisplayName, coinsValue);
                EventManager.FireChangeCoinValue(coinsValue);
                CloseSendCoinsUI();
            }
        }
    }

    public void CloseSendCoinsUI()
    {
        SendCoinsCanvas.gameObject.SetActive(false);
        UserInfo.GetInstance().LockMouse = false;
        UserInfo.GetInstance().LockMovement = false;
    }
    #endregion
}
