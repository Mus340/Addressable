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
    
    private Tween _scoreTween;
    private int _prevScore;

    public GameObject posPanel;
    public GameObject curScorePos;
    public GameObject maxScorePos;
    public GameObject bronzePos;
    public GameObject silverPos;
    public GameObject goldPos;

    private ColorMatchContent _content;
    protected override void Initialize() { }

    protected override void Enter()
    {
        _content = Main.Ins.MainGame.GetGame<ColorMatchContent>();
        _prevScore = _content.Score;
        scoreText.text = $"{_content.Score}";

        maxScorePos.gameObject.SetActive(_content.MaxScore > 0);
        SetPos();
        
        _content.OnAddScore.Subscribe((_) =>
        {          
            SetPos();
            ShowScoreEffect(_content.Score, _prevScore);
            _prevScore = _content.Score;
        }).AddTo(Disposable);
    }

    private void SetPos()
    {
        var curScore = _content.Score;
        var maxScore = 1000;

        var tier = Main.Ins.MainData.RankData.GetTier();
        var rankList = Main.Ins.MainData.RankData.GetRankList(tier);
        var max = Mathf.Max(curScore, maxScore, rankList[0].MaxScore, 1f);

        SetXPositionSmooth(curScorePos, curScore, max);
        SetXPositionSmooth(maxScorePos.gameObject, maxScore, max);
        SetXPositionSmooth(bronzePos.gameObject, rankList[0].MaxScore, max);
        SetXPositionSmooth(silverPos.gameObject, rankList[1].MaxScore, max);
        SetXPositionSmooth(goldPos.gameObject, rankList[2].MaxScore, max);
        
        Debug.Log($"{rankList[0].MaxScore}.{rankList[1].MaxScore}.{rankList[2].MaxScore}");
    }

    private void SetXPositionSmooth(GameObject obj, float score, float maxScore)
    {
        var panelWidth = ((RectTransform)posPanel.transform).rect.width;
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
