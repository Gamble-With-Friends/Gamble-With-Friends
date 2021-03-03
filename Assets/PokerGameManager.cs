using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokerGameManager : MonoBehaviour
{

    bool hasGameStarted;
    GameObject gameCamera;

    private void OnEnable()
    {
        EventManager.onClick += onClick;
        EventManager.onGameStart += onGameStart;
    }

    private void OnDisable()
    {
        EventManager.onClick -= onClick;
        EventManager.onGameStart -= onGameStart;
    }

    void onGameStart(string gameId)
    {
        if (gameId == name)
        {
            gameCamera.SetActive(true);
        }
    }

    void onClick(string gameObjectName)
    {
        if(!hasGameStarted)
        {
            if (gameObjectName == name)
            {
                hasGameStarted = true;
                EventManager.FireGameStartEvent(name);
            }
        }
    }
}
