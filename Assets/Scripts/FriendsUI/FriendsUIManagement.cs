﻿using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendsUIManagement : NetworkBehaviour
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

    public GameObject ChatCanvas;
    public GameObject MessagesScrollViewContent;
    public GameObject NewMessageGroup;
    public GameObject EditMessageGroup;
    public GameObject MessageId;
    public GameObject MessagePrefab;

    private GameObject SearchBarInput;
    private GameObject SearchErrorMessage;

    private UserInfo friend;


    private void OnEnable()
    {
        EventManager.OnFriendRawActionClick += OnFriendRawActionClick;
        EventManager.OnKeyDown += OnKeyDown;
        EventManager.OnRequestChatUpdate += OnRequestChatUpdate;
        EventManager.OnChatUpdate += OnChatUpdate;
    }

    private void OnDisable()
    {
        EventManager.OnFriendRawActionClick -= OnFriendRawActionClick;
        EventManager.OnKeyDown -= OnKeyDown;
        EventManager.OnRequestChatUpdate -= OnRequestChatUpdate;
        EventManager.OnChatUpdate -= OnChatUpdate;
    }

    private void OnKeyDown(KeyCode key)
    {
        if (key == KeyCode.F)
        {
            Debug.Log("Open Friends UI");
            OpenFriendUI();
        }

        else if (key == KeyCode.Return)
        {
            if (ChatCanvas.activeSelf)
            {
                if (NewMessageGroup.activeSelf)
                {
                    SendMessageToFriend();
                }
                else if (EditMessageGroup.activeSelf)
                {
                    UpdateMessage();
                }
            }
        }
    }

    private void OnRequestChatUpdate(string userId)
    {
        CmdRequestChatUpdate(userId);
    }

    private void OnChatUpdate(string userId)
    {
        if (UserInfo.GetInstance().UserId == userId)
        {
            RegenerateMessagesListScrollView();
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
        ClearSearchInputAndMessage();
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
        if (action == FriendRawAction.SendMessage)
        {
            friend = new UserInfo() { UserId = DataManager.GetUserId(displayName), DisplayName = displayName };
            Debug.Log("Open Chat Window");
            OpenChatWindow(displayName);
        }
        if (action == FriendRawAction.SendCoins)
        {
            friend = new UserInfo() { UserId = DataManager.GetUserId(displayName), DisplayName = displayName };
            Debug.Log("Open Send Coins");
            OpenSendCoinsUI(displayName);
        }
        else if (action == FriendRawAction.Unfriend)
        {
            friend = new UserInfo() { UserId = DataManager.GetUserId(displayName), DisplayName = displayName };
            if (UserInfo.GetInstance().UserId != null && displayName != null)
            {
                DataManager.UnfriendUser(UserInfo.GetInstance().UserId, friend.UserId);
                RegenerateFriendListScrollView();
            }
        }
        else if (action == FriendRawAction.SendFriendRequest)
        {
            friend = new UserInfo() { UserId = DataManager.GetUserId(displayName), DisplayName = displayName };
            SendFriendRequest(UserInfo.GetInstance().UserId, displayName);
        }
        else if (action == FriendRawAction.AcceptRequest)
        {
            friend = new UserInfo() { UserId = DataManager.GetUserId(displayName), DisplayName = displayName };
            AcceptFriendRequest(UserInfo.GetInstance().UserId, displayName);
        }
        else if (action == FriendRawAction.DenyRequest)
        {
            friend = new UserInfo() { UserId = DataManager.GetUserId(displayName), DisplayName = displayName };
            DenyFriendRequest(UserInfo.GetInstance().UserId, displayName);
        }
        else if (action == FriendRawAction.EditMessage)
        {
            EditMessage(displayName);
        }
        else if (action == FriendRawAction.DeleteMessage)
        {
            DeleteMessage(displayName);
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

            if (coinsValue < 0)
            {
                GameObject.Find("SendCoinsRawImage/ErrorMessage").GetComponent<Text>().text = "Coin value must be positive";
            }
            else if (!DataManager.GetUserFunds(localUserId, out coinsInWallet))
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
                DataManager.RegisterSendCoinsTransaction(localUserId, friendDisplayName, coinsValue);
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
            SearchBarInput = GameObject.Find("FriendSearchBar");
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
        var searchString = SearchBarInput.GetComponent<InputField>().text;

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
        if (scrollViewContentObj != null && scrollViewContentObj.transform.childCount > 0)
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
            SearchErrorMessage.GetComponent<Text>().color = isError ? Color.red : Color.white;
            SearchErrorMessage.GetComponent<Text>().text = message;
        }
    }

    public void ClearSearchInputAndMessage()
    {
        if (SearchBarInput == null)
        {
            SearchBarInput = GameObject.Find("FriendSearchBar");
        }
        SearchBarInput.GetComponent<InputField>().text = "";
        if (SearchErrorMessage == null)
        {
            SearchErrorMessage = GameObject.Find("FriendSearch/SearchMessageText");
        }
        SearchErrorMessage.GetComponent<Text>().text = "";
    }

    public void OpenChatWindow(string displayName)
    {
        if (UserInfo.GetInstance().UserId != null)
        {
            if (!ChatCanvas.gameObject.activeSelf)
            {
                UserInfo.GetInstance().LockMouse = true;
                UserInfo.GetInstance().LockMovement = true;
                Cursor.lockState = CursorLockMode.Confined;
                ChatCanvas.gameObject.SetActive(true);

                GameObject.Find("MessageInputField").GetComponent<InputField>().Select();

                friend = new UserInfo() { DisplayName = displayName, UserId = DataManager.GetUserId(displayName) };

                GameObject chatWindowTitle = GameObject.Find("ChatWindowTitle");
                if (chatWindowTitle != null)
                {
                    chatWindowTitle.GetComponent<Text>().text = "Chat with " + displayName;
                }
                RegenerateMessagesListScrollView();
            }
        }
    }

    public void CloseChatWindow()
    {
        friend = null;
        ChatCanvas.gameObject.SetActive(false);
    }

    private void RegenerateMessagesListScrollView()
    {
        ClearScrollViewContent(MessagesScrollViewContent);

        var localUserId = UserInfo.GetInstance().UserId;

        if (localUserId == null)
        {
            return;
        }

        var messages = DataManager.GetLastTenMessages(localUserId, friend.UserId);

        InstantiateMessageScrollViewContent(messages);
    }

    public void SendMessageToFriend()
    {        
        string messageContent = GameObject.Find("MessageInputField").GetComponent<InputField>().text;
        if (string.IsNullOrEmpty(messageContent))
        {
            return;
        }
        DataManager.SaveMessage(UserInfo.GetInstance().UserId, friend.DisplayName, messageContent);


        GameObject.Find("MessageInputField").GetComponent<InputField>().text = string.Empty;

        RegenerateMessagesListScrollView();

        EventManager.FireRequestChatUpdate(friend.UserId);
    }

    public void InstantiateMessageScrollViewContent(List<ChatMessage> messages)
    {
        var contentHeight = 5f;
        var messageHeight = MessagePrefab.transform.localScale.y;
        var offset = -100f;

        foreach (var message in messages)
        {
            var messageCard = Instantiate(MessagePrefab, MessagesScrollViewContent.transform); // instantiate prefab

            string title = message.SenderUserId == friend.UserId ? friend.DisplayName + " wrote:" : "You wrote:";
            messageCard.transform.GetChild(0).GetComponent<Text>().text = title;
            messageCard.transform.GetChild(1).GetComponent<Text>().text = message.Content;

            if (message.SenderUserId == friend.UserId)
            {
                messageCard.transform.GetChild(2).gameObject.SetActive(false);
                messageCard.transform.GetChild(3).gameObject.SetActive(false);
            }

            var script = messageCard.GetComponent<MessagePrefabScript>();
            script.MessageId = message.MessageId;

            var localPosition = messageCard.transform.localPosition;
            messageCard.transform.localPosition = new Vector3(localPosition.x, messageHeight + offset, localPosition.z);

            RectTransform rt = (RectTransform)messageCard.transform;
            float height = rt.rect.height;
            offset -= height + 5;

            contentHeight += offset;
        }
    }

    public void EditMessage(string messageId)
    {
        // hide NewMessage group and show editMessage group
        NewMessageGroup.gameObject.SetActive(false);
        EditMessageGroup.gameObject.SetActive(true);
        // load message content
        var message = DataManager.GetMessage(messageId);
        // display message content in the input field
        GameObject.Find("EditMessageInputField").GetComponent<InputField>().text = message;
        GameObject.Find("MessageInputField").GetComponent<InputField>().Select();
        MessageId.GetComponent<Text>().text = messageId;
    }

    public void CancelEdit()
    {
        GameObject.Find("EditMessageInputField").GetComponent<InputField>().text = "";
        MessageId.GetComponent<Text>().text = "";
        EditMessageGroup.gameObject.SetActive(false);
        NewMessageGroup.gameObject.SetActive(true);        
    }

    public void UpdateMessage()
    {
        var messageId = MessageId.GetComponent<Text>().text;
        var content = GameObject.Find("EditMessageInputField").GetComponent<InputField>().text;
        DataManager.UpdateMessage(messageId, content);
        GameObject.Find("EditMessageInputField").GetComponent<InputField>().text = "";
        MessageId.GetComponent<Text>().text = "";
        EditMessageGroup.gameObject.SetActive(false);
        NewMessageGroup.gameObject.SetActive(true);
        RegenerateMessagesListScrollView();
        EventManager.FireRequestChatUpdate(friend.UserId);
    }

    public void DeleteMessage(string messageId)
    {
        DataManager.DeleteMessage(messageId);
        RegenerateMessagesListScrollView();
        EventManager.FireRequestChatUpdate(friend.UserId);
    }


    [Command(requiresAuthority = false)]
    private void CmdRequestChatUpdate(string userId)
    {
        RpcChatUpdate(userId);
    }

    [ClientRpc]
    private void RpcChatUpdate(string userId)
    {
        EventManager.FireChatUpdate(userId);
    }
}
