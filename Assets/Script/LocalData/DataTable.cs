using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;
using UnityEditor;

public class DataTable<T> : DataTable<int,T> where T : struct
{
    
}
public class DataTable<TKey,TValue> where TValue : struct
{
    private Dictionary<TKey, TValue> _data;
    public TValue First => _data.First(p => true).Value;

    public void Load()
    {
        Load(typeof(TValue).Name);
    }

    public void Load(string jsonName, bool isEncrypted = true)
    {
        TextAsset textAsset = Resources.Load<TextAsset>("Data/" + jsonName);

        if (textAsset == null)
        {
            Debug.LogError("JsonDataLoader: Failed to load JSON file: " + jsonName);
            return;
        }

        string decryptedJson = isEncrypted ? CryptoHelper.Decrypt(textAsset.text) : textAsset.text;
        Dictionary<string, object>[] rawData = JsonConvert.DeserializeObject<Dictionary<string, object>[]>(decryptedJson);

        _data = new Dictionary<TKey, TValue>();

        foreach (Dictionary<string, object> rawItem in rawData)
        {
            string itemJson = JsonConvert.SerializeObject(rawItem);
            TValue item = JsonConvert.DeserializeObject<TValue>(itemJson);
            if (typeof(TKey).IsEnum)
            {
                _data.Add((TKey)Enum.Parse(typeof(TKey),rawItem.First().Value.ToString()) , item);
            }
            else
            {
                _data.Add((TKey)Convert.ChangeType(rawItem.First().Value,typeof(TKey)) , item);
            }
            
        }
    }
    
    public bool Contain(TKey id)
    {
        if (_data != null && _data.TryGetValue(id,out var value))
        {
            return true;
        }
        return false;
    }

    public bool TryGetValue(TKey id,out TValue output)
    {
        if (_data != null && _data.TryGetValue(id,out var value))
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

    public TValue GetValue(TKey id)
    {
        if (_data != null && _data.TryGetValue(id,out var value))
        {
            return value;
        }
        else
        {
            Debug.LogError("JsonDataLoader: Invalid id.");
            return default(TValue);
        }
    }

    public TValue? GetValueOrNull(TKey id)
    {
        if (TryGetValue(id, out var value))
        {
            return value;
        }
        else
        {
            return null;
        }
    }

    public int GetLength()
    {
        if (_data != null)
        {
            return _data.Count;
        }
        Debug.LogError("JsonDataLoader: Invalid id.");
        return -1;
    }

    public List<TValue> GetValueList()
    {
        return _data.Values.ToList();
    }
    public IEnumerable<TKey> Keys => _data.Keys;
    public TValue this[TKey key] => GetValue(key);
}