using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public static class JsonLibrary
{
    
    public static string SerializeDictionaryIntString(Dictionary<int, string> dict)
    {
        var keyValues = dict.Select(value => new IntToString {intVal = value.Key, stringVal = value.Value}).ToList();
        var keyValueCollection = new IntToStringCollection {keyValues = keyValues};
        return JsonUtility.ToJson(keyValueCollection);
    }
    
    public static Dictionary<int,string> DeserializeDictionaryIntString(string keyValueCollectionJson)
    {
        if (keyValueCollectionJson == null) return new Dictionary<int, string>();
        var keyValueCollection = JsonUtility.FromJson<IntToStringCollection>(keyValueCollectionJson);
        return keyValueCollection.keyValues.ToDictionary(value => value.intVal, value => value.stringVal);
    } 
    
    public static string SerializeDictionaryIntDecimal(Dictionary<int, decimal> dict)
    {
        var keyValues = dict.Select(value => new IntToDecimal {intVal = value.Key, decimalVal = value.Value}).ToList();
        var keyValueCollection = new IntToDecimalCollection {keyValues = keyValues};
        return JsonUtility.ToJson(keyValueCollection);
    }
    
    public static Dictionary<int,decimal> DeserializeDictionaryIntDecimal(string keyValueCollectionJson)
    {
        if (keyValueCollectionJson == null) return new Dictionary<int, decimal>();
        var keyValueCollection = JsonUtility.FromJson<IntToDecimalCollection>(keyValueCollectionJson);
        return keyValueCollection.keyValues.ToDictionary(value => value.intVal, value => value.decimalVal);
    } 
    
    [Serializable]
    public class IntToString
    {
        public int intVal;
        public string stringVal;
    }

    [Serializable]
    public class IntToStringCollection
    {
        public List<IntToString> keyValues;
    }
    
    [Serializable]
    public class IntToDecimal
    {
        public int intVal;
        public decimal decimalVal;
    }

    [Serializable]
    public class IntToDecimalCollection
    {
        public List<IntToDecimal> keyValues;
    }
}
