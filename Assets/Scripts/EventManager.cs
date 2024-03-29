﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public delegate void PrepareToGameAction(int intanceId);

    public static event PrepareToGameAction OnPrepareToGame;

    public delegate void ReadyToGameAction(int intanceId);

    public static event ReadyToGameAction OnReadyToGame;

    public delegate void PrepareToExitGameAction(int intanceId);

    public static event PrepareToExitGameAction OnPrepareToExitGame;

    public delegate void ReadyToExitGameAction(int intanceId);

    public static event ReadyToExitGameAction OnReadyToExitGame;

    public delegate void StartGame(int intanceId);

    public static event StartGame OnStartGame;

    public delegate void ModifyBetAction(int amount);

    public static event ModifyBetAction OnModifyBetAction;

    public delegate void ClickAction(int instanceId);

    public static event ClickAction OnClick;

    public delegate void InstructionChange(string instruction);

    public static event InstructionChange OnInstructionChange;

    public delegate void LoginSuccess();

    public static event LoginSuccess OnLoginSuccess;

    public delegate void LoginError(string username, string message);

    public static event BeforeLoginSuccess OnBeforeLoginSuccess;

    public delegate void BeforeLoginSuccess();

    public static event LoginError OnLoginError;

    public delegate void ChangeCoinValue(decimal amount);

    public static event ChangeCoinValue OnChangeCoinValue;

    // Event fired when a key is pressed
    public delegate void KeyDownEvents(KeyCode key);

    public static event KeyDownEvents OnKeyDown;

    // Event fired when when coins are clicked
    public delegate void FriendRawActionClicked(FriendRawAction action, string displayName);

    public static event FriendRawActionClicked OnFriendRawActionClick;

    public delegate void OutfitChange(string userId);

    public static event OutfitChange OnOutfitChange;

    public delegate void RequestOutfitChange(string userId);

    public static event RequestOutfitChange OnRequestOutfitChange;

    // request chat changes from server
    public delegate void RequestChatUpdate(string userId);

    public static event RequestChatUpdate OnRequestChatUpdate;

    // request chat changes from user
    public delegate void ChatUpdate(string userId);

    public static event ChatUpdate OnChatUpdate;


    private void OnEnable()
    {
        OnBeforeLoginSuccess += OnBeforeLoginSuccess;
    }

    private void OnDisable()
    {
        OnBeforeLoginSuccess -= OnBeforeLoginSuccess;
    }

    public static void FireClickEvent(int instanceId)
    {
        OnClick?.Invoke(instanceId);
    }

    public static void FirePrepareToGameEvent(int intanceId)
    {
        OnPrepareToGame?.Invoke(intanceId);
    }

    public static void FireReadyToGameEvent(int intanceId)
    {
        OnReadyToGame?.Invoke(intanceId);
    }

    public static void FirePrepareToExitGameEvent(int intanceId)
    {
        OnPrepareToExitGame?.Invoke(intanceId);
    }

    public static void FireReadyToExitGameEvent(int intanceId)
    {
        OnReadyToExitGame?.Invoke(intanceId);
    }

    public static void FireStartGameEvent(int intanceId)
    {
        OnStartGame?.Invoke(intanceId);
    }

    public static void FireOnModifyBetEvent(int amount)
    {
        OnModifyBetAction?.Invoke(amount);
    }

    public static void FireInstructionChangeEvent(string instruction)
    {
        OnInstructionChange?.Invoke(instruction);
    }

    public static void FireLoginSuccessEvent(string username, string userId, decimal coins)
    {
        UserInfo.GetInstance().UserId = userId;
        UserInfo.GetInstance().TotalCoins = coins;
        UserInfo.GetInstance().DisplayName = username;

        OnBeforeLoginSuccess?.Invoke();
    }

    public static void FireDelayedLoginSuccessEvent()
    {
        OnLoginSuccess?.Invoke();
    }

    public static void FireLoginErrorEvent(string username, string errorMessage)
    {
        OnLoginError?.Invoke(username, errorMessage);
    }

    public static void FireChangeCoinValue(decimal amount)
    {
        OnChangeCoinValue?.Invoke(amount);
    }

    public static void FireKeyDownEvent(KeyCode key)
    {
        OnKeyDown?.Invoke(key);
    }

    public static void FireFriendRawActionClick(FriendRawAction action, string displayName)
    {
        OnFriendRawActionClick?.Invoke(action, displayName);
    }

    public static void FireOutfitChange(string userId)
    {
        OnOutfitChange?.Invoke(userId);
    }

    public static void FireRequestOutfitChange(string userId)
    {
        OnRequestOutfitChange?.Invoke(userId);
    }

    public static void FireServer(string userId)
    {
        OnOutfitChange?.Invoke(userId);
    }

    public static void FireRequestChatUpdate(string userId)
    {
        OnRequestChatUpdate?.Invoke(userId);
    }

    public static void FireChatUpdate(string userId)
    {
        OnChatUpdate?.Invoke(userId);
    }
}
