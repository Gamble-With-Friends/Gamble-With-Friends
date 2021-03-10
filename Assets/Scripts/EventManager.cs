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
}
