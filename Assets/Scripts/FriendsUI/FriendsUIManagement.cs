using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendsUIManagement : MonoBehaviour
{
    public GameObject FriendsUICanvas;
    public GameObject friendCardPrefab;

    public GameObject FriendListCanvas;
    public GameObject FriendScrollViewContent;
    public GameObject SendCoinsCanvas;

    public GameObject AddFriendCanvas;
    public GameObject FriendRequestsCanvas;

    void Start()
    {
        //InstantiateFriendCards(transform.position.x, transform.position.y, transform.position.z); // transform.position - position of the parent objectof the script
    }

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

                RegenerateFriendList();
            }
        }
    }

    public void CloseFriendsUI()
    {
        Cursor.lockState = CursorLockMode.Locked;
        OpenFriendListTab();
        FriendsUICanvas.gameObject.SetActive(false);
        UserInfo.GetInstance().LockMouse = false;
        UserInfo.GetInstance().LockMovement = false;
    }

    public void OpenFriendListTab()
    {
        FriendListCanvas.gameObject.SetActive(true);
        AddFriendCanvas.gameObject.SetActive(false);
        FriendRequestsCanvas.gameObject.SetActive(false);
    }

    public void OpenFriendSearchTab()
    {
        FriendListCanvas.gameObject.SetActive(false);
        AddFriendCanvas.gameObject.SetActive(true);
        FriendRequestsCanvas.gameObject.SetActive(false);
    }

    public void OpenPendingRequestsTab()
    {
        FriendListCanvas.gameObject.SetActive(false);
        AddFriendCanvas.gameObject.SetActive(false);
        FriendRequestsCanvas.gameObject.SetActive(true);
    }
    #endregion

    #region Send Coins & Unfriend
    private void OnFriendRawActionClick(FriendRawAction action, string displayName)
    {
        if (action == FriendRawAction.SendCoins)
        {
            Debug.Log("Open Send Coins");
            OpenSendCoinsUI(displayName);
        }
        else if (action == FriendRawAction.Unfriend)
        {
            if (UserInfo.GetInstance().UserId != null && displayName != null)
            {
                DataManager.UnfriendUser(UserInfo.GetInstance().UserId, displayName);
                RegenerateFriendList();
            }
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
                EventManager.FireChangeCoinValue(-coinsValue);
                CloseSendCoinsUI();
            }
        }
    }

    public void CloseSendCoinsUI()
    {
        SendCoinsCanvas.gameObject.SetActive(false);
    }
    #endregion

    private void InstantiateFriendCards(float posX, float posY, float posZ)
    {
        var cardHeight = friendCardPrefab.transform.localScale.y;
        var offset = -30f;

        var localUserId = UserInfo.GetInstance().UserId;

        if (localUserId == null)
        {
            return;
        }

        var friends = DataManager.GetFriends(localUserId);

        foreach (var friend in friends)
        {
            var card = Instantiate(friendCardPrefab, FriendScrollViewContent.transform); // instantiate prefab
            card.GetComponent<FriendCardPrefabScript>().DisplayNameText.text = friend;
            var localPosition = card.transform.localPosition;
            card.transform.localPosition = new Vector3(localPosition.x, cardHeight + offset, localPosition.z);
            offset -= 40;
        }
    }

    private void RegenerateFriendList()
    {
        var friendListScrollView = GameObject.Find("FriendScrollView/Viewport/Content");
        if (friendListScrollView != null && friendListScrollView.transform.childCount > 0)
        {
            for (int i = 0; i < friendListScrollView.transform.childCount; ++i) 
            { 
                Destroy(friendListScrollView.transform.GetChild(i).gameObject); 
            }
        }
        InstantiateFriendCards(transform.position.x, transform.position.y, transform.position.z);
    }
}
