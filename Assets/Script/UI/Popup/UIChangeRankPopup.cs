using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Mosframe;
using UnityEngine;
using UnityEngine.UI;

public class UIChangeRankPopup : UIPopupPanel
{
    public DynamicVScrollView scrollView;

    private Action _onClose;
    public void Open(Tier tier, int prev, int current, Action onclose)
    {
        _onClose = onclose;
        scrollView.totalItemCount = Main.Ins.MainData.RankData.GetRankList(tier).Count;
        gameObject.SetActive(true);
        StartCoroutine(Move(prev, current));
    }

    private IEnumerator Move(int start, int end)
    {
        yield return null;
        scrollView.refresh();
        MoveScrollImmediately(start);
        StartCoroutine(MoveScrollItem(end));
    }
    private void MoveScrollImmediately(int index)
    {
        var scrollRect = scrollView.GetComponent<ScrollRect>();
        var itemHeight = scrollView.itemPrototype.rect.height;
        var viewportHeight = scrollRect.viewport.rect.height;

        var targetY = itemHeight * (index + 0.5f) - viewportHeight / 2f;

        var minY = 0f;
        var maxY = scrollView.totalItemCount * itemHeight - viewportHeight;
        targetY = Mathf.Clamp(targetY, minY, maxY);
        scrollRect.StopMovement();
        scrollRect.content.transform.localPosition = new Vector3(0f, targetY, 0f);
    }
    
    private IEnumerator MoveScrollItem(int index)
    {
        yield return null;
        var scrollRect = scrollView.GetComponent<ScrollRect>();
        var itemHeight = scrollView.itemPrototype.rect.height;
        var viewportHeight = scrollRect.viewport.rect.height;

        var targetY = itemHeight * (index + 0.5f) - viewportHeight / 2f;

        var minY = 0f;
        var maxY = scrollView.totalItemCount * itemHeight - viewportHeight;
        targetY = Mathf.Clamp(targetY, minY, maxY);
        scrollRect.StopMovement();
        scrollRect.content.transform.DOLocalMove(new Vector3(0f, targetY, 0f),1f).SetEase( Ease.OutExpo).OnComplete(
            () =>
            {
                _onClose?.Invoke();
                _onClose = null;
            });
    }
    
}
