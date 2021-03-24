using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

public class PokerServer : NetworkBehaviour
{
    [SyncVar] public GameState gameState;

    [SyncVar(hook = nameof(SetSlotUserId))]
    private string slotToUserIdSerialized;

    public Dictionary<int, string> slotToUserId;
    
    // Public Functions
    public void AddPlayer(string userId)
    {
        CmdAddPlayer(userId);
    }

    public void RemovePlayer(string userId)
    {
        CmdRemovePlayer(userId);
    }

    // Private Functions
    private void SetSlotUserId(string oldValue, string newValue)
    {
        var keyValueCollection = JsonUtility.FromJson<KeyValueCollection>(newValue);
        slotToUserId = keyValueCollection.keyValues.ToDictionary(value => value.slot, value => value.userId);
    }

    [Command(requiresAuthority = false)]
    private void CmdAddPlayer(string userId, NetworkConnectionToClient sender = null)
    {
        var dict = DeserializeDictionary(slotToUserIdSerialized);

        for (var i = 0; i < PokerClient.MaxPlayers; i++)
        {
            if (dict.ContainsKey(i)) continue;
            dict.Add(i, userId);
            break;
        }

        slotToUserIdSerialized = SerializeDictionary(dict);
    }

    [Command(requiresAuthority = false)]
    private void CmdRemovePlayer(string userId, NetworkConnectionToClient sender = null)
    {
        var dict = DeserializeDictionary(slotToUserIdSerialized);

        for (var i = 0; i < PokerClient.MaxPlayers; i++)
        {
            if (!dict.ContainsKey(i) || dict[i] != userId) continue;
            dict.Remove(i);
            break;
        }

        slotToUserIdSerialized = SerializeDictionary(dict);
    }

    private static string SerializeDictionary(Dictionary<int, string> dict)
    {
        var keyValues = dict.Select(value => new KeyValue {slot = value.Key, userId = value.Value}).ToList();
        var keyValueCollection = new KeyValueCollection {keyValues = keyValues};
        return JsonUtility.ToJson(keyValueCollection);
    }
    
    private static Dictionary<int,string> DeserializeDictionary(string keyValueCollectionJson)
    {
        if (keyValueCollectionJson == null) return new Dictionary<int, string>();
        var keyValueCollection = JsonUtility.FromJson<KeyValueCollection>(keyValueCollectionJson);
        return keyValueCollection.keyValues.ToDictionary(value => value.slot, value => value.userId);
    }

    [Command(requiresAuthority = false)]
    private void CmdSetGameState(GameState state)
    {
        gameState = state;
    }

    [Serializable]
    public class KeyValue
    {
        public int slot;
        public string userId;
    }

    [Serializable]
    public class KeyValueCollection
    {
        public List<KeyValue> keyValues;
    }
}