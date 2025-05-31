using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Database;
using UnityEngine;

public class RankData : MonoBehaviour, Data
{    
    public class Rank : ApplyToDictionary<Rank>
    {
        public string UserID;
        public string Name;
        public int Score;
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
            .OrderByChild("Score")
            .LimitToLast(500)
            .GetValueAsync();

        if (snapshot.Exists)
        {
            RankList.Clear();
            foreach (var child in snapshot.Children)
            {
                var data = JsonUtility.FromJson<Rank>(child.GetRawJsonValue());
                data.UserID = child.Key;
                RankList.Add(data);
            }
            RankList = RankList
                .OrderByDescending(data => data.Score)
                .ToList();
        }
    }
    
    public int GetRankNumber(string userID)
    {
        var rank = RankList.OrderByDescending(x => x.Score)
            .TakeWhile(x => x.UserID != userID)  
            .Count();
        return RankList.Any(x => x.UserID == userID) ? rank : -1;
    }
}
