using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class ColorMatchContent : GameContent
{
    public Player Player { get; private set; }
    public Enemy Enemy  { get; private set; }

    private CompositeDisposable _disposable;
    public IObservable<int> OnNext => _onNext;
    private ISubject<int> _onNext = new Subject<int>();
    
    public Transform barParent;
    public ColorCubeBar cubeBar;
    private ObjectPool<ColorCubeBar> _cubeBarPool;
    private Queue<ColorCubeBar> _useCubeQueue;
    
    public List<int> AnswerList {get; private set;}
    public DataTable<LevelData> LevelData { get; private set; } = new();
    
    public int MaxScore {get; private set;}
    public int Score {get; private set;}

    public bool IsEndGame { get; private set; }
    
    public int Level {get; private set;}
    private int _curXPos;
    
    private const int CUBE_RANGE = 30;
    
    public override void Initialized()
    {
        _useCubeQueue = new();
        LevelData.Load();
        _cubeBarPool = new ObjectPool<ColorCubeBar>(cubeBar, CUBE_RANGE, barParent);
    }
    
    public override void Begin()
    {
        _useCubeQueue = new Queue<ColorCubeBar>();
        _disposable = new CompositeDisposable();
        
        AnswerList = new();
        IsEndGame = false;
        Level = 0;
        Score = 0;
        MaxScore = Main.Ins.MainData.UserData.UserInfo.Score;
        _curXPos = 0;
        
        AnswerList.Add(0);
        AnswerList.Add(3);
        for (int i = 2; i < LevelData.GetLength(); i++)
        {
            AnswerList.Add(Random.Range(0, LevelData.GetValue(i).cube_count));
        }
        for (int i = 0; i < CUBE_RANGE; i++)
        {
            var bar = _cubeBarPool.Get();
            bar.SetData(LevelData.GetValue(i), AnswerList[i]);
            _useCubeQueue.Enqueue(bar);
        }
        
        Player = Instantiate(Resources.Load<Player>(ResourcesPath.PlayerPath));
        Player.Initialized(new Vector3(_curXPos, Level, Level));
        Main.Ins.MainCamera.Follow(Player.transform);
        
        var curPlayCount = Main.Ins.MainData.UserData.UserInfo.PlayCount;
        Main.Ins.MainData.UserData.SavePlayCount(++curPlayCount);

        OnNext.Subscribe((level) =>
        {
            if (level == 1)
            {
                Enemy = Instantiate(Resources.Load<Enemy>(ResourcesPath.EnemyPath));
                Enemy.Initialize();
            }
        }).AddTo(_disposable);
    }
    
    public override void End()
    {      
        if (Score > MaxScore)
        {
            MaxScore = Score;
            Main.Ins.MainData.UserData.SaveScore(MaxScore);
            Main.Ins.MainData.RankData.SaveMaxScore(MaxScore);
        }
        foreach (var bar in _useCubeQueue)
        {
            bar.ResetPool();
            _cubeBarPool.ReturnToPool(bar);
        }
        _useCubeQueue.Clear();
        if (Player != null)
        {
            Destroy(Player.gameObject);
        }
        if (Enemy != null)
        {
            Destroy(Enemy.gameObject);
        }
        _disposable?.Dispose();
        _disposable = null;
    }

    private void MoveNext()
    {
        if (_useCubeQueue.Count > CUBE_RANGE + 5)
        {
            var old = _useCubeQueue.Dequeue();
            old.ResetPool();
            _cubeBarPool.ReturnToPool(old);
        }
        var bar = _cubeBarPool.Get();
        bar.SetData(LevelData.GetValue(Level-1+CUBE_RANGE), AnswerList[Level-1+CUBE_RANGE]);
        _useCubeQueue.Enqueue(bar);
        _onNext.OnNext(Level);
    }
    
    
    public void Select(int index)
    {
        if (!IsEndGame)
        {
            if (index == AnswerList[Level+1])
            {
                Success();
            }
            else
            {
                Fail();
            }
        }
    }
    
    private void Success()
    {
        Level++;
        MoveNext();
    }

    public void Fail()
    {
        if (!IsEndGame)
        {
            IsEndGame = true;
            StartCoroutine(FailCoroutine());
        }
    }
    private IEnumerator FailCoroutine()
    {
        yield return new WaitForSeconds(2f);
        var popup = UIMain.Ins.UiPopup.GetPopup<UIColorMatchRetryPopup>(PopupType.ColorMatchRetry);
        popup.Set(Score, MaxScore);
        Main.Ins.MainTime.Pause();
        popup.AddRetryEvent(Main.Ins.MainTime.Resume);
        popup.AddExitEvent(Main.Ins.MainTime.Resume);
        popup.gameObject.SetActive(true);
    }
}
