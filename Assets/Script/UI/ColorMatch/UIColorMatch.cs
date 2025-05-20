using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class UIColorMatch : MonoBehaviour, IDisposable
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI levelText;
    
    private CompositeDisposable _disposable = new CompositeDisposable();

    private void Awake()
    {
        titleText.text = $"Color Match";
    }
    
    private void OnEnable()
    {
        var game = Main.Ins.GameContentProvider.GetGameContent<ColorMatchContent>(GameType.ColorMatch);
        game.OnBegin.Subscribe((_) =>
        {
            levelText.text = $"Level.{game.Level}";
        }).AddTo(_disposable);
        
        game.OnNext.Subscribe((_) =>
        {
            levelText.text = $"Level.{game.Level}";
        }).AddTo(_disposable);
    }

    private void OnDisable()
    {
        Dispose();
    }

    public void Dispose()
    {
        _disposable?.Dispose();
    }
}
