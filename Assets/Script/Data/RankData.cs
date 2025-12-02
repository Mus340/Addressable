using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Database;
using Firebase.Extensions;
using UniRx;
using UnityEngine;

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
    
    public IObservable<(int,int)> OnUpdateRanking => _onUpdateRanking;
    private Subject<(int,int)> _onUpdateRanking = new Subject<(int,int)>();

    private List<Rank> _rankList;
    private DatabaseReference _reference;

    private int _myRankIndex = -1;
    
    public int MyRankIndex
    {
        get => _myRankIndex;
        private set => _myRankIndex = value;
    }
    
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
            _rankList = new List<Rank>();
            foreach (var child in snapshot.Children)
            {
                var data = JsonUtility.FromJson<Rank>(child.GetRawJsonValue());
                data.UserId = child.Key;
                _rankList.Add(data);
            }
            Refresh();
        }
    }

    private void Refresh()
    {
        var prev = MyRankIndex;
        _rankList = _rankList.OrderByDescending(r => r.MaxScore).ToList();
        MyRankIndex = GetRank(Login.Ins.UserID);
        if (prev != MyRankIndex)
        {
            _onUpdateRanking.OnNext((prev, MyRankIndex));
        }
    }

    public async void RemoveUser(string uID)
    {
        var user = _rankList.Find(p => p.UserId == uID);
        if (user != null)
        {
            await Main.Ins.MainData.UserData.SaveScore(0);
            await SaveMaxScore(0);
            _rankList.Remove(user);
            Refresh();
            Debug.Log("Delete User Rank Data");
        }
        else
        {
            Debug.LogError("Cant Find Delete User");
        }
    }
    
    public List<Rank> GetRankList()
    {
        return _rankList;
    }

    public Rank GetRank(int index)
    {
        return _rankList[index];
    }

    public int GetRank(string userID)
    {
        var index = _rankList.FindIndex(p => p.UserId == userID);
        return index;
    }

    public async Task SaveNewUser()
    {
        var data = new Rank();
        data.Name = Main.Ins.MainData.UserData.UserInfo.Name;
        data.MaxScore = Main.Ins.MainData.UserData.UserInfo.Score;
        data.UserId = Login.Ins.UserID;
        await _reference.Child(Login.Ins.UserID).SetValueAsync(data.ToDictionary()).ContinueWithOnMainThread((task) =>
        {
            if (task.IsCompletedSuccessfully == false)
            {
                Debug.LogError($"저장 실패: {task.Exception}");
            }
            else
            {
                _rankList.Add(data);
                Refresh();
            }
        });
    }


    public async Task SaveMaxScore(int maxScore)
    {
        var data = new Dictionary<string, object> {{"MaxScore", maxScore}};
        await _reference.Child(Login.Ins.UserID).UpdateChildrenAsync(data).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompletedSuccessfully == false)
            {
                Debug.LogError($"저장 실패: {task.Exception}");
            }
            else
            {
                _rankList[MyRankIndex].MaxScore = maxScore;
                Refresh();
            }
        });
    }
}
