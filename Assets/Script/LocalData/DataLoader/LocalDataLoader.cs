using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public interface IDataLoader
{
    Task Save(JObject data);
    Task<JObject> Load();
    Task Delete();
}
public class LocalDataLoader : IDataLoader
{
    private string _key;

    public LocalDataLoader(string key)
    {
        _key = key;
    }
    
    public async Task Save(JObject data)
    {
        PlayerPrefs.SetString(_key,data.ToString());
        PlayerPrefs.Save();
    }

    public async Task<JObject> Load()
    {
        var jsonStr = PlayerPrefs.GetString(_key,string.Empty);
        if (string.IsNullOrEmpty(jsonStr) == false)
        {
            var data = JObject.Parse(jsonStr);
            return data;
        }
        else
        {
            return null;
        }
    }
    

    public Task Delete()
    {
        PlayerPrefs.DeleteKey(_key);
        return Task.CompletedTask;
    }
}
