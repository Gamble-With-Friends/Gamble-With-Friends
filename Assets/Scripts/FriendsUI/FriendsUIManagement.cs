using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendsUIManagement : MonoBehaviour
{
    public GameObject FriendsUICanvas;

    public GameObject FriendCardPrefab;
    public GameObject FoundUsersCardPrefab;
    public GameObject PendingRequestPrefab;

    public GameObject FriendListCanvas;
    public GameObject FriendScrollViewContent;
    public GameObject SendCoinsCanvas;

    public GameObject AddFriendCanvas;
    public GameObject SearchScrollViewContent;

    public GameObject FriendRequestsCanvas;
    public GameObject RequestsScrollViewContent;

    private GameObject SearchBarInput;
    private GameObject SearchErrorMessage;

    
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
    
    private void OnKeyDown(KeyCode key)
    {
        if (key == KeyCode.F)
        {
            Debug.Log("Open Friends UI");
            OpenFriendUI();
        }
    }

    #region open/close UI and tabs
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
                RegenerateFriendListScrollView();
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
        RegenerateFriendListScrollView();
    }

    public void OpenFriendSearchTab()
    {
        FriendListCanvas.gameObject.SetActive(false);
        AddFriendCanvas.gameObject.SetActive(true);
        FriendRequestsCanvas.gameObject.SetActive(false);
        ClearScrollViewContent(SearchScrollViewContent);
    }

    public void OpenPendingRequestsTab()
    {
        FriendListCanvas.gameObject.SetActive(false);
        AddFriendCanvas.gameObject.SetActive(false);
        FriendRequestsCanvas.gameObject.SetActive(true);
        RegenerateRequestsListScrollView();
    }
    #endregion

    #region Send Coins, Friend Request & Unfriend
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
                RegenerateFriendListScrollView();
            }
        }
        else if (action == FriendRawAction.SendFriendRequest)
        {
            SendFriendRequest(UserInfo.GetInstance().UserId, displayName);           
        }
        else if (action == FriendRawAction.AcceptRequest)
        {
            AcceptFriendRequest(UserInfo.GetInstance().UserId, displayName);
        }
        else if (action == FriendRawAction.DenyRequest)
        {
            DenyFriendRequest(UserInfo.GetInstance().UserId, displayName);
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

    public void SearchUser()
    {
        ClearScrollViewContent(SearchScrollViewContent);

        if (SearchBarInput == null)
        {
            SearchBarInput = GameObject.Find("FriendSearchBar/Text");
        }
        if (SearchErrorMessage == null)
        {
            SearchErrorMessage = GameObject.Find("FriendSearch/SearchMessageText");
        }
        if (SearchBarInput == null)
        {
            if (SearchErrorMessage != null)
            {
                SearchErrorMessage.GetComponent<Text>().text = "Unexpected error occured";
            }
            return;
        }
        var searchString = SearchBarInput.GetComponent<Text>().text;

        if (string.IsNullOrEmpty(searchString))
        {
            DisplaySearchMessage("Please provide search input", true);
            return;
        }

        if (searchString.Length < 2)
        {
            DisplaySearchMessage("At least 2 letters required for search", true);
            return;
        }

        var foundUsers = DataManager.FindBySearchString(searchString, UserInfo.GetInstance().DisplayName);
        if (foundUsers == null || foundUsers.Count == 0)
        {
            DisplaySearchMessage("No users found", true);
        }        

        InstantiateScrollViewContent(SearchScrollViewContent, FoundUsersCardPrefab, foundUsers);
    }

    private void SendFriendRequest(string currentUserId, string otherUserDisplayName)
    {
        if (DataManager.IsFriend(currentUserId, otherUserDisplayName))
        {
            DisplaySearchMessage("This user already is your friend", true);
        }
        else if (DataManager.FriendRequestAlreadySent(currentUserId, otherUserDisplayName))
        {
            DisplaySearchMessage("You've already sent sent request to this user", true);
        }
        else
        {
            DataManager.SendFriendRequest(currentUserId, otherUserDisplayName);
            DisplaySearchMessage("Friend request was sent to " + otherUserDisplayName, false);
        }       
    }
    
    private void AcceptFriendRequest(string currentUserId, string otherUserDisplayName)
    {
        DataManager.AcceptFriendRequest(currentUserId, otherUserDisplayName);
        RegenerateRequestsListScrollView();
    }

    private void DenyFriendRequest(string currentUserId, string otherUserDisplayName)
    {
        DataManager.RemoveFriendRequest(currentUserId, otherUserDisplayName);
        RegenerateRequestsListScrollView();
    }

    private void ClearScrollViewContent(GameObject scrollViewContentObj)
    {
        if (scrollViewContentObj!= null && scrollViewContentObj.transform.childCount > 0)
        {
            for (int i = 0; i < scrollViewContentObj.transform.childCount; ++i) 
            { 
                Destroy(scrollViewContentObj.transform.GetChild(i).gameObject); 
            }
        }
    }

    private void InstantiateScrollViewContent(GameObject scrollViewContentObj, GameObject prefab, List<string> users)
    {
        var cardHeight = prefab.transform.localScale.y;
        var offset = -30f;

        foreach (var user in users)
        {
            var card = Instantiate(prefab, scrollViewContentObj.transform); // instantiate prefab
            card.transform.GetChild(0).GetComponent<Text>().text = user;
            var localPosition = card.transform.localPosition;
            card.transform.localPosition = new Vector3(localPosition.x, cardHeight + offset, localPosition.z);
            offset -= 40;
        }
    }

    private void RegenerateFriendListScrollView()
    {
        ClearScrollViewContent(FriendScrollViewContent);

        var localUserId = UserInfo.GetInstance().UserId;

        if (localUserId == null)
        {
            return;
        }

        var friends = DataManager.GetFriends(localUserId);

        InstantiateScrollViewContent(FriendScrollViewContent, FriendCardPrefab, friends);
    }

    private void RegenerateRequestsListScrollView()
    {
        ClearScrollViewContent(RequestsScrollViewContent);
        List<string> pendingRequests = DataManager.GetPendingFriendRequests(UserInfo.GetInstance().UserId);

        InstantiateScrollViewContent(RequestsScrollViewContent, PendingRequestPrefab, pendingRequests);
    }

    public void DisplaySearchMessage(string message, bool isError)
    {
        if (SearchErrorMessage != null)
        {
            SearchErrorMessage.GetComponent<Text>().color = isError? Color.red : Color.white;
            SearchErrorMessage.GetComponent<Text>().text = message;
        }
    }            
}
