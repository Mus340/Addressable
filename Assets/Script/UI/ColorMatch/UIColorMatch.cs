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
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI maxScoreText;


    public Slider timerSlider;
    
    protected override void Initialize()
    {
        titleText.text = $"Color Match";
    }

    protected override void Enter()
    {
        var game = Main.Ins.MainGame.GameContentProvider.GetGameContent<ColorMatchContent>(GameType.ColorMatch);
        scoreText.text = $"{game.Score}";
        maxScoreText.text = $"{game.MaxScore}";
        game.OnNext.Subscribe((_) =>
        {
            scoreText.text = $"{game.Score}";
            maxScoreText.text = $"{game.MaxScore}";
        }).AddTo(Disposable);

        timerSlider.maxValue = game.TIMER_TIME;
        timerSlider.value = game.TIMER_TIME;
        
        game.TimeLeft
            .Subscribe(value => timerSlider.value = value)
            .AddTo(this);
    }
}
