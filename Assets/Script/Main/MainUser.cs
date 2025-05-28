using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;

public class MainUser : MonoBehaviour
{
    private UserData _userData = new();
    private string userID;
    
    public async Task Initialize()
    {
        userID = Login.Ins.UserId;
        if (Login.Ins.IsNewUser)
        {
            await Main.Ins.MainData.SaveAsync(userID,  _userData.ToDictionary());
        }
        else
        {
            _userData = await Main.Ins.MainData.LoadUserData(userID);
        }
    }

    public int GetPlayCount()
    {
        return _userData.PlayCount;
    }

    public int GetScore()
    {
        return _userData.Score;
    }

    public string GetName()
    {
        return _userData.Name;
    }

    public int GetRank()
    {
        return Main.Ins.MainData.GetRank(userID);
    }
    
    public void SavePlayCount(int count)
    {
        _userData.PlayCount = count;
        Main.Ins.MainData.Save(userID, new Dictionary<string, object>
        {
            { "PlayCount", _userData.PlayCount }
        });
    }
    public void SaveScore(int score)
    {
        _userData.Score = score;
        Main.Ins.MainData.Save(userID, new Dictionary<string, object>
        {
            { "Score", _userData.Score }
        });
    }
}
