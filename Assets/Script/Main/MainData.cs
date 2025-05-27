using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;

public class UserData
{
    public string Uid;
    public string Name;
    public int PlayCount;
    public int Score;
}

public class MainData : MonoBehaviour
{
    private DatabaseReference _reference;
    public List<UserData> UserRankList { get; private set; } = new();
    
    private void Awake()
    {
        _reference = FirebaseDatabase.DefaultInstance.GetReference("User");
    }

    public async Task Initialize()
    {
        try
        {
            var dependencyResult = await FirebaseApp.CheckAndFixDependenciesAsync();
            if (dependencyResult == DependencyStatus.Available)
            {
                await LoadRankUserData();
                await Main.Ins.MainUser.Load();
            }
            else
            {
                Debug.LogError($"Firebase 초기화 실패: {dependencyResult}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Firebase 초기화 실패: {e}");
        }
    }
    
    private async Task LoadRankUserData()
    {
        var snapshot = await _reference
            .OrderByChild("Score")
            .LimitToLast(500)
            .GetValueAsync();

        if (snapshot.Exists)
        {
            UserRankList.Clear();
            foreach (var child in snapshot.Children)
            {
                var data = JsonUtility.FromJson<UserData>(child.GetRawJsonValue());
                data.Uid = child.Key;
                UserRankList.Add(data);
            }
            UserRankList = UserRankList
                .OrderByDescending(data => data.Score)
                .ToList();
        }
    }

    public async Task<UserData> LoadUserData(string uID)
    {
        var snapshot = await _reference.Child(uID).GetValueAsync();
        UserData data = null;
        if (snapshot.Exists)
        {
            data = JsonUtility.FromJson<UserData>(snapshot.GetRawJsonValue());
            data.Uid = uID;
        }
        return data;
    }

    public void Save(string uID, Dictionary<string, object> data)
    {
        _reference.Child(uID).UpdateChildrenAsync(data).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompletedSuccessfully == false)
            {
                Debug.LogError($"저장 실패: {task.Exception}");
            }
        });
    }

    public int GetRank(string uID)
    {
        var rank = UserRankList
            .OrderByDescending(x => x.Score)
            .TakeWhile(x => x.Uid != uID)  
            .Count();
        return UserRankList.Any(x => x.Uid == uID) ? rank : -1;
    }

}
