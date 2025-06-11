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
    public GameObject enemyPos;
    public UIRankPos uIRankPosOrigin;
    private List<UIRankPos> _rankPosList;

    private ColorMatchContent _content;
    protected override void Initialize()
    {
        _rankPosList = new();
        for (int i = 0; i < Main.Ins.MainData.RankData.GetRankList().Count; i++)
        {
            var item = Instantiate(uIRankPosOrigin, uIRankPosOrigin.transform.parent);
            item.transform.SetAsFirstSibling();
            item.Initialize(i);
            _rankPosList.Add(item);
        }
        uIRankPosOrigin.gameObject.SetActive(false);
    }

    protected override void Enter()
    {
        _content = Main.Ins.MainGame.GetGame<ColorMatchContent>();
        _prevScore = _content.Score;
        scoreText.text = $"{_content.Score}";

        foreach (var rankItem in _rankPosList)
        {
            rankItem.gameObject.SetActive(true);
        }
        maxScorePos.gameObject.SetActive(_content.MaxScore > 0);
        SetPos();
        _content.OnNext.Subscribe((_) =>
        {
            SetPos();
            ShowScoreEffect(_content.Score, _prevScore);
            _prevScore = _content.Score;

        }).AddTo(Disposable);
        
        _content.OnAddScore.Subscribe((_) =>
        {          
            SetPos();
            ShowScoreEffect(_content.Score, _prevScore);
            _prevScore = _content.Score;
        }).AddTo(Disposable);
        
        _content.SpawnEnemy.Subscribe((_) =>
        {
            _content.Enemy.OnNext.Subscribe((_) =>
            {
                SetPos();
            }).AddTo(Disposable);
        }).AddTo(Disposable);
    }

    private void SetPos()
    {
        var curScore = _content.Score;
        var maxScore = _content.MaxScore;
        var enemyScore = 0;
        if (_content.Enemy != null)
        {
            enemyScore = _content.Enemy.Pos.y;
        }
        
        var rankList = Main.Ins.MainData.RankData.GetRankList();
        var max = Mathf.Max(curScore, maxScore, rankList[0].MaxScore, 1f);
        
        SetXPositionSmooth(curScorePos, curScore, max);
        SetXPositionSmooth(maxScorePos.gameObject, maxScore, max);
        SetXPositionSmooth(enemyPos, enemyScore, _content.Player.GetPos().y);
        
        for (int i = 0; i < _rankPosList.Count; i++)
        {
            if (curScore >= rankList[i].MaxScore)
            {
                _rankPosList[i].gameObject.gameObject.SetActive(false);
            }
        }
        
        for (int i = 0; i < _rankPosList.Count; i++)
        {
            SetXPositionSmooth(_rankPosList[i].gameObject, rankList[i].MaxScore, max);
        }
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
