using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class UIColorMatch : UIContentPanel
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI maxLevelText;


    public Slider timerSlider;
    
    protected override void Initialize()
    {
        titleText.text = $"Color Match";
    }

    protected override void Enter()
    {
        Debug.Log("UIColorMatch Enter");
        var game = Main.Ins.MainGame.GameContentProvider.GetGameContent<ColorMatchContent>(GameType.ColorMatch);
        levelText.text = $"Level {game.Level}";
        maxLevelText.text = $"Level {game.MaxLevel}";
        game.OnNext.Subscribe((_) =>
        {
            levelText.text = $"Level {game.Level}";
            maxLevelText.text = $"Level {game.MaxLevel}";
        }).AddTo(Disposable);
        
        timerSlider.maxValue = ColorMatchData.TimerTime;
        timerSlider.value = ColorMatchData.TimerTime;
        game.TimeLeft
            .Subscribe(value => timerSlider.value = value)
            .AddTo(this);
    }
}
