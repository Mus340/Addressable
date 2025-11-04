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
    
    public IObservable<Unit> OnUpdateMaxScore => _onUpdateMaxScore;
    private Subject<Unit> _onUpdateMaxScore = new Subject<Unit>();
    
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
        _rankList = _rankList.OrderByDescending(r => r.MaxScore).ToList();
        MyRankIndex = GetRank(Login.Ins.UserID);
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

    public void SaveNewUser()
    {
        var data = new Rank();
        data.Name = Main.Ins.MainData.UserData.UserInfo.Name;
        data.MaxScore = Main.Ins.MainData.UserData.UserInfo.Score;
        data.UserId = Login.Ins.UserID;
        _reference.Child(Login.Ins.UserID).SetValueAsync(data.ToDictionary()).ContinueWithOnMainThread((task) =>
        {
            if (task.IsCompletedSuccessfully == false)
            {
                Debug.LogError($"저장 실패: {task.Exception}");
            }
            else
            {
                _rankList.Add(data);
                Refresh();
                _onUpdateMaxScore.OnNext(Unit.Default);
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
            else
            {
                _rankList[MyRankIndex].MaxScore = maxScore;
                Refresh();
                _onUpdateMaxScore.OnNext(Unit.Default);
            }
        });
    }
}
