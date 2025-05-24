using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

public class ColorMatchContent : GameContent
{
    public IObservable<int> OnNext => _onNext;
    private ISubject<int> _onNext = new Subject<int>();
    
    private IDisposable _timerDisposable;
    public ReactiveProperty<float> TimeLeft {get; private set;}
    
    public ColorMatchItem colorMatchItem;
    private Dictionary<int,ColorMatchItem> _colorMatchItems;
    private int _answerIndex;
    private bool _isEndGame;
    private int _level;
    
    public int MaxScore {get; private set;}
    public int Score {get; private set;}

    private readonly int ITEM_COUNT = 4; 
    public readonly int TIMER_TIME = 5;
    
    public override void Initialized()
    {
        colorMatchItem.gameObject.SetActive(false);
        _colorMatchItems = new Dictionary<int, ColorMatchItem>();
        for (int i = 0; i < ITEM_COUNT; i++)
        {
            var item = Instantiate(colorMatchItem, colorMatchItem.transform.parent);
            item.gameObject.SetActive(true);
            item.SetIndex(i);
            _colorMatchItems.Add(i, item);
        }
    }
    
    public override void Begin()
    {
        TimeLeft = new ReactiveProperty<float>();
        _timerDisposable = new CompositeDisposable();

        _isEndGame = false;
        _level = 1;
        Score = 0;
        MaxScore = Main.Ins.MainData.GetData<int>(nameof(GameType.ColorMatch),"MaxScore");
        
        StartStage(_level);
        StartTimer(TIMER_TIME);
    }
    
    public override void End()
    {
        StopTimer();
    }
    
    private void StartStage(int level)
    {
        TimeLeft.Value = TIMER_TIME;
        _answerIndex = Random.Range(0, _colorMatchItems.Count);
        var color = GetColor(level);
        foreach (var item in _colorMatchItems)
        {
            item.Value.SetItem(color);
        }
        var fadeFactor = Mathf.Lerp(1f, 0.05f, Mathf.Log(level, 50));
        var lerpColor = Color.Lerp(color, Color.white, fadeFactor);
        _colorMatchItems[_answerIndex].SetItem(lerpColor);
    }
    private void MoveNextStage()
    {
        StartStage(_level);
        _onNext.OnNext(_level);
    }

    public void Select(int select)
    {
        if (!_isEndGame)
        {
            if (_answerIndex == select)
            {
                Success();
            }
            else
            {
                ShowAnswer(select);
                StartCoroutine(Fail());
            }
        }
    }
    
    private void Success()
    {
        _level++;
        Score += _level + (int)TimeLeft.Value;
        
        Debug.Log($"{_level}.{(int)TimeLeft.Value}.{_level + (int)TimeLeft.Value}");
        if (Score > MaxScore)
        {
            MaxScore = Score;
            Main.Ins.MainData.SetData(nameof(GameType.ColorMatch), "MaxScore", MaxScore);
            Main.Ins.MainData.Save();
        }
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

    private void ShowAnswer(int? select = null)
    {
        if (select == null)
        {
            _colorMatchItems[_answerIndex].ShowSuccess();
        }
        else
        {
            _colorMatchItems[select.Value].ShowFail();
            _colorMatchItems[_answerIndex].ShowSuccess();
        }
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
                    ShowAnswer();
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

    private Color GetColor(int level)
    {
        var color = Color.white;
        switch ((level-1) % 7)
        {
            case 0: color = new Color(1f, 0.4f, 0.4f); break;
            case 1: color = new Color(1f, 0.6f, 0.3f); break;
            case 2: color = new Color(1f, 1f, 0.5f); break;
            case 3: color = new Color(0.5f, 1f, 0.5f); break;
            case 4: color = new Color(0.5f, 0.7f, 1f); break;
            case 5: color = new Color(0.6f, 0.5f, 1f); break;
            case 6: color = new Color(0.8f, 0.5f, 1f); break;
        }
        return color;
    }
}
