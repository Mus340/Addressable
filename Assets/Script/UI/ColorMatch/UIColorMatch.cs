using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class UIColorMatch : UIContentPanel
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI maxScoreText;
    public TextMeshProUGUI timerText;

    public GameObject sliderPanel;
    public Slider scoreSlider;
    private Tween _scoreTween;
    
    private int _prevScore;
    protected override void Initialize()
    {
        titleText.text = $"Color Match";
    }

    protected override void Enter()
    {
        var game = Main.Ins.MainGame.GameContentProvider.GetGameContent<ColorMatchContent>(GameType.ColorMatch);
        _prevScore = game.Score;
        scoreText.text = $"{game.Score}";
        maxScoreText.text = $"{game.MaxScore}";
        scoreSlider.maxValue = game.MaxScore;
        scoreSlider.value = game.Score;

        sliderPanel.SetActive(game.MaxScore > 0);
        game.OnNext.Subscribe((_) =>
        {
            ShowScoreEffect(game.Score, _prevScore);
            _prevScore = game.Score;

        }).AddTo(Disposable);
        
        timerText.text = $"{game.TIMER_TIME}";
        game.TimeLeft
            .Subscribe(value =>
            {
                timerText.text = $"{(int)value + 1}";
            })
            .AddTo(this);
    }

    private void ShowScoreEffect(int curScore, int prevScore)
    {
        _scoreTween?.Kill();
        _scoreTween = DOVirtual.Int(
            from: prevScore,
            to: curScore,
            duration: 0.5f,
            onVirtualUpdate: value =>
            {
                scoreText.text = value.ToString("N0");
                scoreSlider.value = value;
            }
        );
    }
}
