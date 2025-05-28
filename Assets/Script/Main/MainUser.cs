using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;

public class MainUser : MonoBehaviour
{
    private UserData _userData;
    private string _uID;

    public void SetUid(string uId)
    {
        this._uID = uId;
        Debug.Log(this._uID);
    }
    public async Task Load()
    {
        _userData = await Main.Ins.MainData.LoadUserData(_uID);
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
        return Main.Ins.MainData.GetRank(_uID);
    }
    
    public void SavePlayCount(int count)
    {
        _userData.PlayCount = count;
        Main.Ins.MainData.Save(_uID, new Dictionary<string, object>
        {
            { "PlayCount", _userData.PlayCount }
        });
    }
    public void SaveScore(int score)
    {
        _userData.Score = score;
        Main.Ins.MainData.Save(_uID, new Dictionary<string, object>
        {
            { "Score", _userData.Score }
        });
    }
}
