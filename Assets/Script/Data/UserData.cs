using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;



public class UserData : MonoBehaviour
{
    public class User : ApplyToDictionary<User>
    {
        public string Name;
        public int PlayCount;
        public int Score;
        public string FirstTime;
        public string LastTime;
    }

    public User UserInfo { get; private set; } = new();
    private DatabaseReference _reference;
    
    public async Task Initialize(FirebaseDatabase reference)
    {
        _reference = reference.GetReference("User");
        await CheckUser();
    }

    private async Task CheckUser()
    {
        if (Login.Ins.IsNewUser)
        {
            UserInfo.FirstTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            UserInfo.LastTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            await SaveAsync(UserInfo.ToDictionary());
        }
        else
        {
            await Load();
            SaveLastTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        }
    }

    private async Task Load()
    {
        var uID = Login.Ins.UserID;
        var snapshot = await _reference.Child(uID).GetValueAsync();
        if (snapshot.Exists)
        {
            UserInfo = JsonUtility.FromJson<User>(snapshot.GetRawJsonValue());
        }
    }

    private void Save(Dictionary<string, object> data)
    {
        var uID = Login.Ins.UserID;
        _reference.Child(uID).UpdateChildrenAsync(data).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompletedSuccessfully == false)
            {
                Debug.LogError($"저장 실패: {task.Exception}");
            }
        });
    }

    private async Task SaveAsync(Dictionary<string, object> data)
    {
        try
        {
            var uID = Login.Ins.UserID;
            await _reference.Child(uID).UpdateChildrenAsync(data);
        }
        catch (Exception e)
        {
            Debug.LogError($"저장 실패: {e}");
        }
    }
    public void SaveLastTime(string nowTime)
    {
        UserInfo.LastTime = nowTime;
        Save(new Dictionary<string, object> {{nameof(User.LastTime), UserInfo.LastTime}});
    }
    public void SaveName(string nameStr)
    {
        UserInfo.Name = nameStr;
        Save(new Dictionary<string, object> {{nameof(User.Name), UserInfo.Name}});
    }

    public void SavePlayCount(int count)
    {
        UserInfo.PlayCount = count;
        Save(new Dictionary<string, object> {{nameof(User.PlayCount), UserInfo.PlayCount}});
    }
    
    public void SaveScore(int score)
    {
        UserInfo.Score = score;
        Save(new Dictionary<string, object> {{nameof(User.Score), UserInfo.Score}});
    }

}
