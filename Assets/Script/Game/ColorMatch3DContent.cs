using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class ColorMatch3DContent : GameContent
{
    public Player player;
    
    private CompositeDisposable _disposable;
    
    public IObservable<int> OnNext => _onNext;
    private ISubject<int> _onNext = new Subject<int>();
    
    private IDisposable _timerDisposable;
    public ReactiveProperty<float> TimeLeft {get; private set;}
    
    public Transform barParent;
    public ColorCubeBar cubeBar;
    private ObjectPool<ColorCubeBar> _cubeBarPool;

    private Queue<ColorCubeBar> _useCubeQueue;
    
    private List<int> _answerList;
    private DataTable<LevelData> _levelData = new();
    public int MaxScore {get; private set;}
    public int Score {get; private set;}

    private bool _isEndGame;
    private int _level;
    
    public readonly int TIMER_TIME = 500;
    private const int CUBE_RANGE = 30;
    public override void Initialized()
    {
        _useCubeQueue = new();
        _levelData.Load();
        _cubeBarPool = new ObjectPool<ColorCubeBar>(cubeBar, CUBE_RANGE, barParent);
    }
    
    public override void Begin()
    {
        player = Instantiate(Resources.Load<Player>(ResourcesPath.PlayerPath));
        
        _useCubeQueue = new Queue<ColorCubeBar>();
        TimeLeft = new ReactiveProperty<float>();
        _timerDisposable = new CompositeDisposable();
        _disposable = new CompositeDisposable();
        
        _isEndGame = false;
        _level = 0;
        Score = 0;
        MaxScore = Main.Ins.MainData.UserData.UserInfo.Score;

        _curXPos = 0;
        
        _answerList = new();
        for (int i = 0; i < _levelData.GetLength(); i++)
        {
            _answerList.Add(Random.Range(0, _levelData.GetValue(i).block_count));
        }
        for (int i = 0; i < CUBE_RANGE; i++)
        {
            var bar = _cubeBarPool.Get();
            bar.Initialize();
            bar.SetData(_levelData.GetValue(i), _answerList[i]);
            _useCubeQueue.Enqueue(bar);
        }
        
        StartStage(_level);
        StartTimer(TIMER_TIME);
        
        player.Move(new Vector3(_curXPos, _level+player.transform.localScale.y+1, _level));
    }
    
    public override void End()
    {      
        if (Score > MaxScore)
        {
            MaxScore = Score;
            Main.Ins.MainData.UserData.SaveScore(MaxScore);
            Main.Ins.MainData.RankData.SaveMaxScore(MaxScore);
        }
        var curPlayCount = Main.Ins.MainData.UserData.UserInfo.PlayCount;
        Main.Ins.MainData.UserData.SavePlayCount(++curPlayCount);
        StopTimer();
        foreach (var bar in _useCubeQueue)
        {
            bar.ResetPool();
            _cubeBarPool.ReturnToPool(bar);
        }
        if (player != null)
        {
            Destroy(player.gameObject);
        }
        _disposable.Dispose();
        _disposable = null;
    }

    private void StartStage(int level)
    {
        TimeLeft.Value = TIMER_TIME;

        if (_useCubeQueue.Count > CUBE_RANGE + 5)
        {
            var old = _useCubeQueue.Dequeue();
            old.ResetPool();
            _cubeBarPool.ReturnToPool(old);
        }
        var bar = _cubeBarPool.Get();
        bar.Initialize();
        bar.SetData(_levelData.GetValue(level+CUBE_RANGE), _answerList[level+CUBE_RANGE]);
        _useCubeQueue.Enqueue(bar);
    }

    private void MoveNextStage()
    {
        StartStage(_level);
        _onNext.OnNext(_level);
    }

    private void Move(PlayerMove move)
    {
        if (move == PlayerMove.Left && (_curXPos-1) >= 0)
        {
            _curXPos--;
        }
        else if (move == PlayerMove.Right && (_curXPos + 1) < _levelData.GetValue(_level).block_count)
        {
            _curXPos++;
        }
        player.Move(new Vector3(_curXPos, _level+player.transform.localScale.y, _level));
    }
    private void Select(int level, int index)
    {
        if (!_isEndGame)
        {
            Debug.Log($"{level}.{index}");
            if (index == _answerList[level+1])
            {
                Success();
            }
            else
            {
                StartCoroutine(Fail());
            }
        }
    }
    
    private void Success()
    {
        _level++;
        Score += _level + (int)TimeLeft.Value;
        player.Move(new Vector3(_curXPos, _level+player.transform.localScale.y, _level));
        MoveNextStage();
    }
    
    private IEnumerator Fail()
    {
        _isEndGame = true;
        StopTimer();
        yield return new WaitForSeconds(2f);
        var popup = UIMain.Ins.UiPopup.GetPopup<UIColorMatchRetryPopup>(PopupType.ColorMatchRetry);
        popup.Set(Score, MaxScore);
        Main.Ins.MainTime.Pause();
        popup.AddRetryEvent(Main.Ins.MainTime.Resume);
        popup.AddExitEvent(Main.Ins.MainTime.Resume);
        popup.gameObject.SetActive(true);
    }
    
    private void StartTimer(float time)
    {
        TimeLeft.Value = time;
        _timerDisposable = Observable
            .Interval(TimeSpan.FromSeconds(0.01f))
            .TakeWhile(_ => TimeLeft.Value > 0)
            .Subscribe(_ =>
            {
                TimeLeft.Value -= 0.01f;
                TimeLeft.Value = Mathf.Max(TimeLeft.Value, 0f);

                if (TimeLeft.Value <= 0)
                {
                    StartCoroutine(Fail());
                }
            })
            .AddTo(this);
    }

    private void StopTimer()
    {
        TimeLeft?.Dispose();
        TimeLeft = null;
        _timerDisposable?.Dispose();
        _timerDisposable = null;
    }


    #region TodoMove
    private int _curXPos;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Move(PlayerMove.Left);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            Move(PlayerMove.Right);
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            Select(_level, _curXPos);
        }
    }
    #endregion
}
