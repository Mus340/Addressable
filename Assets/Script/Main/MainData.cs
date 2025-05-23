using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;

public static class FirebaseConfig
{
    public static string DatabaseUrl = "https://firebase-database.firebaseio.com/";
    public static string Test_ID = "Test";
}
public class MainData : MonoBehaviour
{
    private Dictionary<string, Dictionary<string, object>> _userData = new();
    private Dictionary<string, Dictionary<string, object>> _dirtyData = new();
    private DatabaseReference _reference;
    
    public async Task Initialize()
    {
        _reference = FirebaseDatabase.DefaultInstance.RootReference;

        var dependencyResult = await FirebaseApp.CheckAndFixDependenciesAsync();

        if (dependencyResult == DependencyStatus.Available)
        {
            await LoadUserData();
        }
        else
        {
            Debug.LogError($"Firebase 초기화 실패: {dependencyResult}");
        }
    }

    
    private async Task LoadUserData()
    {
        try
        {
            var snapshot = await _reference.Child("UserData").Child(FirebaseConfig.Test_ID).GetValueAsync();
            foreach (var content in snapshot.Children)
            {
                if (!_userData.ContainsKey(content.Key))
                {
                    _userData[content.Key] = new Dictionary<string, object>();
                }

                foreach (var entry in content.Children)
                {
                    _userData[content.Key][entry.Key] = entry.Value;
                }
            }
            Debug.Log("UserData Load Complete");
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
    }


    public T GetData<T>(string type, string key)
    {
        if (_userData.TryGetValue(type, out var contentDict)) 
        {
            if (contentDict.TryGetValue(key, out var rawValue))
            {
                try
                {
                    return (T)Convert.ChangeType(rawValue, typeof(T));
                }
                catch
                {
                    Debug.LogError($"Can't convert {key} to type {typeof(T).Name}");
                }   
            }
        }
        return default;
    }

    public void SetData(string type, string key, object value)
    {   
        if (!_userData.ContainsKey(type))
        {
            _userData[type] = new Dictionary<string, object>();
        }
        _userData[type][key] = value;

        if (!_dirtyData.ContainsKey(type))
        {
            _dirtyData[type] = new Dictionary<string, object>();
        }
        _dirtyData[type][key] = value;
    }

    public void Save()
    {
        if (_dirtyData.Count == 0)
        {
            return;
        }
        var userRef = FirebaseDatabase.DefaultInstance.GetReference("UserData").Child(FirebaseConfig.Test_ID);
        foreach (var typePair in _dirtyData)
        {
            var type = typePair.Key;
            var data = typePair.Value;

            userRef.Child(type).UpdateChildrenAsync(data).ContinueWithOnMainThread(task =>
            {
                if (task.IsCompletedSuccessfully == false)
                {
                    Debug.LogError($"[{type}] 저장 실패: {task.Exception}");
                }
            });
        }
        _dirtyData.Clear();
    }

    
}
