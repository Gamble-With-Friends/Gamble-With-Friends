using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

public class PokerServer : NetworkBehaviour
{
    public PokerClient client;

    [SyncVar] public decimal totalBets;

    [SyncVar(hook = nameof(SyncGameState))]
    public GameState gameState;

    [SyncVar(hook = nameof(SyncSeatToUserId))]
    private string slotToUserIdSerialized;
    
    [SyncVar(hook = nameof(SyncTurn))]
    private int turn;

    // Public Functions
    public void AddPlayer(string userId)
    {
        CmdAddPlayer(userId);
    }

    public void RemovePlayer(string userId)
    {
        CmdRemovePlayer(userId);
    }

    // Sync Var Functions
    private void SyncSeatToUserId(string oldValue, string newValue)
    {
        client.OnSeatToUserIdChange(oldValue, newValue);
    }

    private void SyncGameState(GameState oldValue, GameState newValue)
    {
        client.OnGameStateChange(oldValue.ToString(), newValue.ToString());
    }

    private void SyncTurn(int oldValue, int newValue)
    {
        client.OnTurnChange(newValue);
    }

    private void SetGameState(Dictionary<int,string> dict)
    {
        // If not enough players, set the game state to waiting for players
        if (dict.Count < 2)
        {
            gameState = GameState.WaitingForPlayers;
        }
        // If more than 1 player and waiting for players, set the game state to betting
        else if (gameState == GameState.WaitingForPlayers)
        {
            gameState = GameState.Betting;
        }
    }

    [Command(requiresAuthority = false)]
    private void CmdAddPlayer(string userId, NetworkConnectionToClient sender = null)
    {
        var dict = JsonLibrary.DeserializeDictionaryIntString(slotToUserIdSerialized);

        for (var i = 0; i < PokerClient.MaxPlayers; i++)
        {
            if (dict.ContainsKey(i)) continue;
            dict.Add(i, userId);
            break;
        }

        var oldGameState = gameState;
        SetGameState(dict);
        
        if (oldGameState == GameState.WaitingForPlayers && gameState == GameState.Betting)
        {
            turn = 0;
        }
        
        slotToUserIdSerialized = JsonLibrary.SerializeDictionaryIntString(dict);
    }

    [Command(requiresAuthority = false)]
    private void CmdRemovePlayer(string userId, NetworkConnectionToClient sender = null)
    {
        var dict = JsonLibrary.DeserializeDictionaryIntString(slotToUserIdSerialized);

        for (var i = 0; i < PokerClient.MaxPlayers; i++)
        {
            if (!dict.ContainsKey(i) || dict[i] != userId) continue;
            dict.Remove(i);
            break;
        }

        SetGameState(dict);
        slotToUserIdSerialized = JsonLibrary.SerializeDictionaryIntString(dict);
    }


    [Command(requiresAuthority = false)]
    public void CmdSetGameState(GameState state)
    {
        gameState = state;
    }
}