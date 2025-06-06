using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;
using UnityEditor;

public class DataArray<TValue> where TValue : struct
{
    public TValue[] Values => data.ToArray();
    private List<TValue> data;
    public void Load()
    {
        Load(typeof(TValue).Name);
    }

    public void Load(string jsonName)
    {
        TextAsset textAsset = Resources.Load<TextAsset>("Data/" + jsonName);

        if (textAsset == null)
        {
            Debug.LogError("JsonDataLoader: Failed to load JSON file: " + jsonName);
            return;
        }

        string encryptedJson = textAsset.text;
        string decryptedJson = CryptoHelper.Decrypt(encryptedJson);
        Dictionary<string, object>[] rawData = JsonConvert.DeserializeObject<Dictionary<string, object>[]>(decryptedJson);

        var loadData = new List<TValue>();
        foreach (Dictionary<string, object> rawItem in rawData)
        {
            string itemJson = JsonConvert.SerializeObject(rawItem);
            TValue item = JsonConvert.DeserializeObject<TValue>(itemJson);
            loadData.Add(item);
        }

        data = loadData;
    }
    public int GetLength()
    {
        if (data != null)
        {
            return data.Count;
        }
        Debug.LogError("JsonDataLoader: Invalid id.");
        return -1;
    }
}