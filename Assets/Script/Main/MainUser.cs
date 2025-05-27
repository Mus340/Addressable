using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class MainUser : MonoBehaviour
{
    private readonly string Test_ID = "Test";
    private UserData _userData;

    public async Task Load()
    {
        _userData = await Main.Ins.MainData.LoadUserData(Test_ID);
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
        return Main.Ins.MainData.GetRank(Test_ID);
    }
    
    public void SavePlayCount(int count)
    {
        _userData.PlayCount = count;
        Main.Ins.MainData.Save(Test_ID, new Dictionary<string, object>
        {
            { "PlayCount", _userData.PlayCount }
        });
    }
    public void SaveScore(int score)
    {
        _userData.Score = score;
        Main.Ins.MainData.Save(Test_ID, new Dictionary<string, object>
        {
            { "Score", _userData.Score }
        });
    }
}
