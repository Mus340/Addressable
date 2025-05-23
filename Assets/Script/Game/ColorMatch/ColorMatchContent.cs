using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

public static class ColorMatchData
{
    public static int TimerTime = 5;
    
}
public class ColorMatchContent : GameContent
{
    public IObservable<int> OnNext => _onNext;
    private ISubject<int> _onNext = new Subject<int>();
    
    private IDisposable _timerDisposable;
    public ReactiveProperty<float> TimeLeft {get; private set;}

    
    public ColorMatchItem colorMatchItem;
    private Dictionary<int,ColorMatchItem> _colorMatchItems;
    private int _answerIndex;
    public int Level {get; private set;}

    private readonly int TEMP_ITEM_COUNT = 4;
    
    public override void Initialized()
    {
        Level = 1;
        
        colorMatchItem.gameObject.SetActive(false);
        _colorMatchItems = new Dictionary<int, ColorMatchItem>();
        for (int i = 0; i < TEMP_ITEM_COUNT; i++)
        {
            var item = Instantiate(colorMatchItem, colorMatchItem.transform.parent);
            item.gameObject.SetActive(true);
            item.SetIndex(i);
            _colorMatchItems.Add(i, item);
        }
    }

    public override void End()
    {
        foreach (var item in _colorMatchItems)
        {
            Destroy(item.Value.gameObject);
        }
        _colorMatchItems.Clear();
        
        TimeLeft?.Dispose();
        TimeLeft = null;
        _timerDisposable?.Dispose();
        _timerDisposable = null;
    }
    public override void Begin()
    {
        TimeLeft = new ReactiveProperty<float>();
        _timerDisposable = new CompositeDisposable();
        
        StartStage(Level);
        StartTimer(ColorMatchData.TimerTime);
    }

    private void StartStage(int level)
    {
        TimeLeft.Value = ColorMatchData.TimerTime;
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
        Level++;
        StartStage(Level);
        _onNext.OnNext(Level);
    }

    public void Select(int select)
    {
        if (_answerIndex == select)
        {
            Success();
        }
        else
        {
            Fail();
        }
    }
    
    private void Success()
    {
        MoveNextStage();
    }
    
    private void Fail()
    {
        Main.Ins.MainGame.ReturnToLobby();
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
                    Fail();
                }
            })
            .AddTo(this);
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
