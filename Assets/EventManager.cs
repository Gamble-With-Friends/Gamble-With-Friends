using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{

    public delegate void GameStartAction(string gameId);
    public static event GameStartAction onGameStart;

    public delegate void ClickAction(string gameObjectName);
    public static event ClickAction onClick;


    public static void FireClickEvent(string gameObjectName)
    {
        onClick?.Invoke(gameObjectName);
    }

    public static void FireGameStartEvent(string gameId)
    {
        onGameStart?.Invoke(gameId);
    }

}
