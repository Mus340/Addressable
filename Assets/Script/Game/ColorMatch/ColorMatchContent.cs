using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using Random = UnityEngine.Random;


public class ColorMatchContent : GameContent
{
    private enum ColorType
    {
        Red, Orange, Yellow, Green, Blue, Indigo, Violet
    }
    
    public IObservable<int> OnNext => _onNext;
    private ISubject<int> _onNext = new Subject<int>();
    
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
        _onEnd.OnNext(new Void());
    }
    public override void Begin()
    {
        _onBegin.OnNext(Void.Empty);
        StartStage(Level);
    }

    public override void Subscribe()
    {
        
    }

    private void StartStage(int level)
    {
        _answerIndex = Random.Range(0, _colorMatchItems.Count);
        var color = GetRandomColor();
        foreach (var item in _colorMatchItems)
        {
            item.Value.SetItem(color);
        }
        _colorMatchItems[_answerIndex].SetItem(Color.white);
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
            Debug.Log("Success");
        }
        else
        {
            Fail();
            Debug.Log("Fail");
        }
    }
    
    private void Success()
    {
        MoveNextStage();
    }
    
    private void Fail()
    {
        Main.Ins.ReturnToLobby();
    }
    
    private Color GetRandomColor()
    {
        var selectColor = (ColorType)Random.Range(0, Enum.GetValues(typeof(ColorType)).Length);
        var baseColor = Color.white;
        switch (selectColor)
        {
            case ColorType.Red:    baseColor = new Color(1f, 0.4f, 0.4f); break;
            case ColorType.Orange: baseColor = new Color(1f, 0.6f, 0.3f); break;
            case ColorType.Yellow: baseColor = new Color(1f, 1f, 0.5f); break;
            case ColorType.Green:  baseColor = new Color(0.5f, 1f, 0.5f); break;
            case ColorType.Blue:   baseColor = new Color(0.5f, 0.7f, 1f); break;
            case ColorType.Indigo: baseColor = new Color(0.6f, 0.5f, 1f); break;
            case ColorType.Violet: baseColor = new Color(0.8f, 0.5f, 1f); break;
        }
        return baseColor;
    }
}
