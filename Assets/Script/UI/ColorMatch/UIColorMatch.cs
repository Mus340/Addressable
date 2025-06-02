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
    public Text scoreText;
    public Text timerText;

    private Tween _scoreTween;
    
    private int _prevScore;

    public GameObject posPanel;
    public GameObject curScorePos;
    public GameObject maxScorePos;
    public UIRankPos uIRankPosOrigin;
    private List<UIRankPos> _rankPosList;
    
    protected override void Initialize()
    {
        _rankPosList = new();
        for (int i = 0; i < Main.Ins.MainData.RankData.RankList.Count; i++)
        {
            var item = Instantiate(uIRankPosOrigin, uIRankPosOrigin.transform.parent);
            item.transform.SetAsFirstSibling();
            item.Set(i);
            _rankPosList.Add(item);
        }
        uIRankPosOrigin.gameObject.SetActive(false);
    }

    protected override void Enter()
    {
        var game = Main.Ins.MainGame.GameContentProvider.GetGameContent<ColorMatchContent>(GameType.ColorMatch);
        _prevScore = game.Score;
        scoreText.text = $"{game.Score}";
        
        maxScorePos.SetActive(game.MaxScore > 0);
        
        SetPos();
        game.OnNext.Subscribe((_) =>
        {
            SetPos();
            ShowScoreEffect(game.Score,_prevScore);
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
    
    private void SetPos()
    {
        var content = Main.Ins.MainGame.GameContentProvider.GetGameContent<ColorMatchContent>(GameType.ColorMatch);
        var curScore = content.Score;
        var maxScore = content.MaxScore;

        float max = Mathf.Max(curScore, maxScore, Main.Ins.MainData.RankData.RankList[0].MaxScore, 1f);
        float panelWidth = ((RectTransform)posPanel.transform).rect.width;

        SetXPositionSmooth(curScorePos, curScore, max, panelWidth);
        SetXPositionSmooth(maxScorePos, maxScore, max, panelWidth);

        for (int i = 0; i < Main.Ins.MainData.RankData.RankList.Count; i++)
        {
            SetXPositionSmooth(_rankPosList[i].gameObject, Main.Ins.MainData.RankData.RankList[i].MaxScore, max, panelWidth);
        }
    }
    
    private void SetXPositionSmooth(GameObject obj, float score, float maxScore,float panelWidth)
    {
        var normalized = Mathf.Clamp01(score / maxScore);
        var rt = obj.GetComponent<RectTransform>();
        rt.DOAnchorPos(new Vector2(normalized * panelWidth, 0), 0.3f)
            .SetEase(Ease.OutQuad);
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
            }
        );
    }
}
