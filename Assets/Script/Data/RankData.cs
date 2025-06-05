using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;

public enum Tier
{
    Brown = default,
    Yellow,
    Green,
    Emerald,
    Blue,
    Pink,
    Red,
    Purple,
}

public class RankData : MonoBehaviour
{
    public class Rank : ApplyToDictionary<Rank>
    {
        public string Name;
        public int MaxScore;
    }

    public Sprite[] tierSprite;
    private Dictionary<Tier, List<Rank>> _rankDictionary;
    private DatabaseReference _reference;
    
    private List<int> _rankUserRange = new List<int>();
    
    public async Task Initialize(FirebaseDatabase reference)
    {
        _reference = reference.GetReference("Rank");
        await LoadRanker();
    }
    
    public async Task LoadRanker()
    {
        var snapshot = await _reference.GetValueAsync();

        if (snapshot.Exists)
        {
            _rankDictionary = new Dictionary<Tier, List<Rank>>();
            var allRanks = new List<Rank>();

            foreach (var child in snapshot.Children)
            {
                var data = JsonUtility.FromJson<Rank>(child.GetRawJsonValue());
                allRanks.Add(data);
            }
            SetTierRange(allRanks.Count);
            allRanks = allRanks.OrderByDescending(r => r.MaxScore).ToList();

            for (int i = 0; i < allRanks.Count; i++)
            {
                var tierIndex = _rankUserRange.FindIndex(range => i < range);
                if (tierIndex == -1)
                {
                    tierIndex = _rankUserRange.Count - 1;
                }
                var tier = (Tier)tierIndex;

                if (!_rankDictionary.ContainsKey(tier))
                {
                    _rankDictionary[tier] = new List<Rank>();
                }

                _rankDictionary[tier].Add(allRanks[i]);
            }
        }
    }
    
    private void SetTierRange(int total)
    {
        int tierCount = Enum.GetValues(typeof(Tier)).Length;

        _rankUserRange.Clear();
        int groupSize = Mathf.CeilToInt((float)total / tierCount);
        int sum = 0;

        for (int i = 0; i < tierCount; i++)
        {
            sum += groupSize;
            if (sum > total)
            {
                sum = total;
            }
            _rankUserRange.Add(sum);
        }
    }
    
    public Tier GetTier()
    {
        return Tier.Blue;
    }
    public List<Rank> GetRankList()
    {
        return _rankDictionary[Tier.Blue];
    }
    public int GetRankNumber()
    {
        return 1;
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
