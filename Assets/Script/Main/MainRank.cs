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


public class RankData
{
    public string UserName;
    public int PlayCount;
    public int Score;

    public string ToString()
    {
        return $"{UserName}.{PlayCount}.{Score}";
    }
}

public class MainRank : MonoBehaviour
{
    public List<RankData> RankList { get; private set; } = new();
    private DatabaseReference _reference;
    
    public async Task Load()
    {
        _reference = FirebaseDatabase.DefaultInstance.GetReference("UserRanking");

        var snapshot = await _reference
            .OrderByChild("Score")
            .LimitToLast(500)
            .GetValueAsync();

        if (snapshot.Exists)
        {
            RankList.Clear();
            foreach (var child in snapshot.Children)
            {
                var data = JsonUtility.FromJson<RankData>(child.GetRawJsonValue());
                RankList.Add(data);
            }
            RankList = RankList
                .OrderByDescending(data => data.Score)
                .ToList();
        }
    }
}
