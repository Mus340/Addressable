using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;

public class RankData : MonoBehaviour
{    
    public class Rank : ApplyToDictionary<Rank>
    {
        public string Name;
        public int MaxScore;
    }

    public List<Rank> RankList { get; private set; } = new();
    private DatabaseReference _reference;
    
    public async Task Initialize(FirebaseDatabase reference)
    {
        _reference = reference.GetReference("Rank");
        await LoadRankUserData();
    }
    
    private async Task LoadRankUserData()
    {
        var snapshot = await _reference
            .OrderByChild("MaxScore")
            .LimitToLast(500)
            .GetValueAsync();

        if (snapshot.Exists)
        {
            RankList.Clear();
            foreach (var child in snapshot.Children)
            {
                var data = JsonUtility.FromJson<Rank>(child.GetRawJsonValue());
                RankList.Add(data);
            }
            RankList = RankList
                .OrderByDescending(data => data.MaxScore)
                .ToList();
        }
    }
    
    public int GetRankNumber(string userName) 
    {
        var rank = RankList
            .OrderByDescending(x => x.MaxScore)
            .TakeWhile(x => x.Name != userName)
            .Count();

        return RankList.Any(x => x.Name == userName) ? rank : -1;
    }
    
    public void SaveInfo(string userName)
    {
        try
        {
            var data = new Rank();
            data.Name = userName;
            _reference.Child(Login.Ins.UserID).SetValueAsync(data.ToDictionary()).ContinueWithOnMainThread((task) =>
            {
                if (task.IsCompletedSuccessfully == false)
                {
                    Debug.LogError($"저장 실패: {task.Exception}");
                }
            });
        }
        catch (Exception e)
        {
            Debug.LogError($"저장 실패: {e}");
        }
    }
    
    public void SaveMaxScore(int maxScore)
    {
        var data = new Dictionary<string, object> {{"MaxScore", maxScore}};
        _reference.Child(Login.Ins.UserID).UpdateChildrenAsync(data).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompletedSuccessfully == false)
            {
                Debug.LogError($"저장 실패: {task.Exception}");
            }
        });
    }
}
