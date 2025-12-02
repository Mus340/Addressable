using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Mosframe;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class UIRankingPopup : UIPopupPanel
{
    public UIMyRankItem MyRankItem;
    public DynamicVScrollView ScrollView;

    private bool _isMoveScroll;
    private void Awake()
    {
        if (Main.Ins.LoadComplete)
        {
            Initialize();
        }
        else
        {
            Main.Ins.OnLoadComplete.Subscribe((_) =>
            {
                Initialize();
            }).AddTo(this);
        } 
    }

    private void Initialize()
    {
        _isMoveScroll = true;
        ScrollView.totalItemCount = Main.Ins.MainData.RankData.GetRankList().Count;

        Main.Ins.MainData.RankData.OnUpdateRanking.Subscribe((rank) =>
        {
            Debug.Log($"Change Ranking : prev.{rank.Item1}, cur.{rank.Item2}");
            if (rank.Item1 == -1 || rank.Item2 == -1)
            {
                ScrollView.totalItemCount = Main.Ins.MainData.RankData.GetRankList().Count;
            }
            _isMoveScroll = true;
        }).AddTo(this);
    }

    public void Open()
    {
        SetScroll();
        MyRankItem.Set();
    }

    private void SetScroll()
    {
        if (_isMoveScroll)
        {
            StartCoroutine(Move(Main.Ins.MainData.RankData.MyRankIndex));
        }
    }


    private IEnumerator Move(int index)
    {
        yield return null;
        ScrollView.refresh();
        StartCoroutine(MoveScrollItem(index));
        _isMoveScroll = false;                                                                                                             
    }
    
    private IEnumerator MoveScrollItem(int index)
    {
        yield return null;
        var scrollRect = ScrollView.GetComponent<ScrollRect>();
        var itemHeight = ScrollView.itemPrototype.rect.height;
        var viewportHeight = scrollRect.viewport.rect.height;

        var targetY = itemHeight * (index + 0.5f) - viewportHeight / 2f;

        var minY = 0f;
        var maxY = ScrollView.totalItemCount * itemHeight - viewportHeight;
        targetY = Mathf.Clamp(targetY, minY, maxY);
        scrollRect.StopMovement();
        scrollRect.content.transform.DOLocalMove(new Vector3(0f, targetY+(scrollRect.viewport.rect.height/2), 0f),1f).SetEase( Ease.OutExpo).OnComplete(
            () =>
            {
            });
    }

}
