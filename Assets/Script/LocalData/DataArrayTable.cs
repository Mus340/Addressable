using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;
using UnityEditor;

public class DataArrayTable<T> : DataArrayTable<long,T> where T : struct
{
    
}
public class DataArrayTable<TKey,TValue> where TValue : struct
{
    private Dictionary<TKey, TValue[]> data;
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

        var loadData = new Dictionary<TKey, List<TValue>>();
        foreach (Dictionary<string, object> rawItem in rawData)
        {
            string itemJson = JsonConvert.SerializeObject(rawItem);
            TValue item = JsonConvert.DeserializeObject<TValue>(itemJson);

            TKey key = !typeof(TKey).IsEnum?(TKey)Convert.ChangeType(rawItem.First().Value,typeof(TKey)):(TKey)Enum.Parse(typeof(TKey),rawItem.First().Value.ToString());
            if (loadData.TryGetValue(key, out var list))
            {
                list.Add(item);
            }
            else
            {
                loadData.Add(key,new List<TValue>(){item});
            }
        }
        data = loadData.ToDictionary((p)=>p.Key,p=>p.Value.ToArray());
    }
    public bool Contain(TKey id)
    {
        if (data != null && data.TryGetValue(id,out var value))
        {
            return true;
        }
        return false;
    }

    public bool TryGetValue(TKey id,out TValue[] output)
    {
        if (data != null && data.TryGetValue(id,out var value))
        {
            output = value;
            return true;
        }
        else
        {
            output = default;
            return false;
        }
    }

    public TValue[] GetValue(TKey id)
    {
        if (data != null && data.TryGetValue(id,out var value))
        {
            return value;
        }
        else
        {
            return new TValue[]{};
        }
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

    public List<TValue[]> GetValueList()
    {
        return data.Values.ToList();
    }
    public IEnumerable<TKey> Keys => data.Keys;
    public TValue[] this[TKey key] => GetValue(key);

    public TValue? FirstOrNull(TKey id,System.Func<TValue, bool> predicate)
    {
        var values = this[id].Where(predicate).ToArray();
        return values.Any() ? values.First() : null;
    }
}