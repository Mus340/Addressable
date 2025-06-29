using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Database;
using Firebase.Extensions;
using UniRx;
using UnityEngine;

public enum Tier
{
    Brown,
    Yellow,
    Green,
    Emerald,
    Blue,
    Red,
    Purple,
}

public class RankData : MonoBehaviour
{
    public class Rank : ApplyToDictionary<Rank>
    {
        public string UserId { get; set; }
        public string Name;
        public int MaxScore;
    }

    public bool LoadComplete { get; private set; }
    public IObservable<Unit> OnLoadComplete => _onLoadComplete;
    private Subject<Unit> _onLoadComplete = new Subject<Unit>();

    public Sprite[] tierSprite;
    private Dictionary<Tier, List<Rank>> _rankDictionary;
    private DatabaseReference _reference;


    public async Task Initialize(FirebaseDatabase reference)
    {
        LoadComplete = false;
        _reference = reference.GetReference("Rank");
        await LoadRanker();
        LoadComplete = true;
        _onLoadComplete.OnNext(Unit.Default);
    }

    private async Task LoadRanker()
    {
        var snapshot = await _reference.GetValueAsync();

        if (snapshot.Exists)
        {
            var rangeList = new List<int>();
            _rankDictionary = new Dictionary<Tier, List<Rank>>();
            var allRanks = new List<Rank>();

            foreach (var child in snapshot.Children)
            {
                var data = JsonUtility.FromJson<Rank>(child.GetRawJsonValue());
                data.UserId = child.Key;
                allRanks.Add(data);
            }

            allRanks = allRanks.OrderByDescending(r => r.MaxScore).ToList();

            var tierCount = Enum.GetValues(typeof(Tier)).Length;
            rangeList.Clear();
            var groupSize = Mathf.CeilToInt((float) allRanks.Count / tierCount);
            var sum = 0;

            for (int i = 0; i < tierCount; i++)
            {
                sum += groupSize;
                if (sum > allRanks.Count)
                {
                    sum = allRanks.Count;
                }

                rangeList.Add(sum);
            }

            for (int i = 0; i < allRanks.Count; i++)
            {
                var tierIndex = rangeList.FindIndex(range => i < range);
                if (tierIndex == -1)
                {
                    tierIndex = rangeList.Count - 1;
                }

                int reversedTierIndex = (rangeList.Count - 1) - tierIndex;
                var tier = (Tier) reversedTierIndex;

                if (!_rankDictionary.ContainsKey(tier))
                {
                    _rankDictionary[tier] = new List<Rank>();
                }

                _rankDictionary[tier].Add(allRanks[i]);
            }
        }
    }

    public Tier GetTier(int score)
    {
        Tier? matchedTier = null;

        foreach (var kvp in _rankDictionary)
        {
            var tier = kvp.Key;
            var ranks = kvp.Value;

            int highest = ranks[0].MaxScore;
            int lowest = ranks[ranks.Count - 1].MaxScore;

            if (score <= highest && score >= lowest)
            {
                matchedTier = tier;
                break;
            }

            if (score > highest)
            {
                matchedTier = tier+1;
                if (matchedTier > Tier.Purple)
                {
                    matchedTier = Tier.Purple;
                }
                break;
            }
        }
        return matchedTier ?? _rankDictionary.Keys.Min();
    }

    public void Remove(Tier prevTier)
    {
        var userId = Login.Ins.UserID;
        if (_rankDictionary.TryGetValue(prevTier, out var rankList))
        {
            var target = rankList.FirstOrDefault(rank => rank.UserId == userId);
            if (target != null)
            {
                rankList.Remove(target);
            }
        }
    }

    public void Add(Tier tier)
    {
        var userId = Login.Ins.UserID;
        var newRank = new Rank
        {
            UserId = userId,
            Name = Main.Ins.MainData.UserData.UserInfo.Name,
            MaxScore = Main.Ins.MainData.UserData.UserInfo.Score,
        };
        
        if (!_rankDictionary.ContainsKey(tier))
        {
            _rankDictionary[tier] = new List<Rank>();
        }
        var rankList = _rankDictionary[tier];
        var existing = rankList.FirstOrDefault(rank => rank.UserId == userId);
        if (existing != null)
        {
            rankList.Remove(existing);
        }
        rankList.Add(newRank);
        rankList.Sort((a, b) => b.MaxScore.CompareTo(a.MaxScore));
    }

    public void Refresh(Tier tier)
    {
        if (!_rankDictionary.ContainsKey(tier))
        {
            _rankDictionary[tier] = new List<Rank>();
        }
        var rankList = _rankDictionary[tier];
        rankList.Sort((a, b) => b.MaxScore.CompareTo(a.MaxScore));
    }
    
    public List<Rank> GetRankList(Tier tier)
    {
        return _rankDictionary[tier];
    }

    public Rank GetRankData(Tier tier, int index)
    {
        return _rankDictionary[tier][index];
    }

    public int GetRankNumber()
    {
        var tier = Enum.Parse<Tier>(Main.Ins.MainData.UserData.UserInfo.Tier);
        var index = _rankDictionary[tier].FindIndex(p => p.UserId == Login.Ins.UserID);
        return index;
    }

    public void SaveAll()
    {
        var data = new Rank();
        data.Name = Main.Ins.MainData.UserData.UserInfo.Name;
        data.MaxScore = Main.Ins.MainData.UserData.UserInfo.Score;
        _reference.Child(Login.Ins.UserID).SetValueAsync(data.ToDictionary()).ContinueWithOnMainThread((task) =>
        {
            if (task.IsCompletedSuccessfully == false)
            {
                Debug.LogError($"저장 실패: {task.Exception}");
            }
        });
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
