using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;

public class MainUser : MonoBehaviour
{
    public static UserData UserDataConfig = new();
    
    public async Task Initialize()
    {
        if (Login.Ins.IsNewUser)
        {
            await Main.Ins.MainData.SaveAsync(UserDataConfig.Uid, UserDataConfig.ToDictionary());
        }
        else
        {
            UserDataConfig = await Main.Ins.MainData.LoadUserData(UserDataConfig.Uid);
        }
    }

    public int GetPlayCount()
    {
        return UserDataConfig.PlayCount;
    }

    public int GetScore()
    {
        return UserDataConfig.Score;
    }

    public string GetName()
    {
        return UserDataConfig.Name;
    }

    public int GetRank()
    {
        return Main.Ins.MainData.GetRank(UserDataConfig.Uid);
    }

    public void SaveName(string nameStr)
    {
        UserDataConfig.Name = nameStr;
        Main.Ins.MainData.Save(UserDataConfig.Uid, new Dictionary<string, object>
        {
            { "Name", UserDataConfig.Name }
        });
    }
    
    public void SavePlayCount(int count)
    {
        UserDataConfig.PlayCount = count;
        Main.Ins.MainData.Save(UserDataConfig.Uid, new Dictionary<string, object>
        {
            { "PlayCount", UserDataConfig.PlayCount }
        });
    }
    public void SaveScore(int score)
    {
        UserDataConfig.Score = score;
        Main.Ins.MainData.Save(UserDataConfig.Uid, new Dictionary<string, object>
        {
            { "Score", UserDataConfig.Score }
        });
    }
}
