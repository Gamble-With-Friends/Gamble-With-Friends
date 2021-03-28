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

    public delegate void LoginSuccess(string username, string userId, decimal coins);

    public static event LoginSuccess OnLoginSuccess;
    
    public delegate void LoginError(string username, string message);

    public static event LoginError OnLoginError;
    
    public delegate void ChangeCoinValue(decimal amount);

    public static event ChangeCoinValue OnChangeCoinValue;

    // Event fired when a key is pressed
    public delegate void KeyDownEvents(KeyCode key);

    public static event KeyDownEvents OnKeyDown;

    // Event fired when when coins are clicked
    public delegate void FriendRawActionClicked(FriendRawAction action, string displayName);

    public static event FriendRawActionClicked OnFriendRawActionClick;

    public delegate void OutfitChange();

    public static event OutfitChange OnOutfitChange;

    public delegate void StringValueChanged(EventType eventType, string oldValue, string newValue);

    public static event StringValueChanged OnStringValueChange;

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
        OnLoginSuccess?.Invoke(username, userId, coins);
    }
    
    public static void FireLoginErrorEvent(string username, string errorMessage)
    {
        OnLoginError?.Invoke(username, errorMessage);
    }
    
    public static void FireChangeCoinValue(decimal amount)
    {
        OnChangeCoinValue?.Invoke(amount);
    }

    //
    public static void FireKeyDownEvent(KeyCode key)
    {
        OnKeyDown?.Invoke(key);
    }

    public static void FireFriendRawActionClick(FriendRawAction action, string displayName)
    {
        OnFriendRawActionClick?.Invoke(action, displayName);
    }   
    
    public static void FireStringValueChange(EventType eventType, string oldValue, string newValue)
    {
        OnStringValueChange?.Invoke(eventType, oldValue, newValue);
    }

    public static void FireOutfitChange()
    {
        OnOutfitChange?.Invoke();
    }
}