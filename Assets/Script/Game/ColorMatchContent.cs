using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

public class ColorMatchContent : GameContent
{
    private Player _player;
    private Enemy _enemy;
    
    public IObservable<int> OnNext => _onNext;
    private ISubject<int> _onNext = new Subject<int>();
    
    public Transform barParent;
    public ColorCubeBar cubeBar;
    private ObjectPool<ColorCubeBar> _cubeBarPool;
    private Queue<ColorCubeBar> _useCubeQueue;
    
    private List<int> _answerList;
    public DataTable<LevelData> LevelData { get; private set; }= new();
    
    public int MaxScore {get; private set;}
    public int Score {get; private set;}

    public bool IsEndGame { get; private set; }
    
    public int _level;
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
        
        _answerList = new();
        IsEndGame = false;
        _level = 0;
        Score = 0;
        MaxScore = Main.Ins.MainData.UserData.UserInfo.Score;
        _curXPos = 0;
        
        for (int i = 0; i < LevelData.GetLength(); i++)
        {
            _answerList.Add(Random.Range(0, LevelData.GetValue(i).cube_count));
        }
        for (int i = 0; i < CUBE_RANGE; i++)
        {
            var bar = _cubeBarPool.Get();
            bar.SetData(LevelData.GetValue(i), _answerList[i]);
            _useCubeQueue.Enqueue(bar);
        }
        
        _player = Instantiate(Resources.Load<Player>(ResourcesPath.PlayerPath));
        _player.Initialized(new Vector3(_curXPos, _level, _level));
        Main.Ins.MainCamera.Follow(_player.transform);
        
        var curPlayCount = Main.Ins.MainData.UserData.UserInfo.PlayCount;
        Main.Ins.MainData.UserData.SavePlayCount(++curPlayCount);

        OnNext.Take(1).Subscribe((level) =>
        {
            if (level == 1)
            {
                _enemy = Instantiate(Resources.Load<Enemy>(ResourcesPath.EnemyPath));
                _enemy.Initialize();
            }
        });
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
        if (_player != null)
        {
            Destroy(_player.gameObject);
        }
        if (_enemy != null)
        {
            Destroy(_enemy.gameObject);
        }
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
        bar.SetData(LevelData.GetValue(_level-1+CUBE_RANGE), _answerList[_level-1+CUBE_RANGE]);
        _useCubeQueue.Enqueue(bar);
        _onNext.OnNext(_level);
    }
    
    
    public void Select(int index)
    {
        if (!IsEndGame)
        {
            if (index == _answerList[_level+1])
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
        MoveNext();
    }
    
    private IEnumerator Fail()
    {
        IsEndGame = true;
        yield return new WaitForSeconds(2f);
        var popup = UIMain.Ins.UiPopup.GetPopup<UIColorMatchRetryPopup>(PopupType.ColorMatchRetry);
        popup.Set(Score, MaxScore);
        Main.Ins.MainTime.Pause();
        popup.AddRetryEvent(Main.Ins.MainTime.Resume);
        popup.AddExitEvent(Main.Ins.MainTime.Resume);
        popup.gameObject.SetActive(true);
    }
}
