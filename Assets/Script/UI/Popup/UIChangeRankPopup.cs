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
    public Button closeButton;
    private Action _onClose;

    public int curRank;
    public int prevRank;
    public void Open(int prev, int cur, Action onClose)
    {
        this.prevRank = prev;
        this.curRank = cur;
        closeButton.gameObject.SetActive(false);
        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(()=>
        {
            StopAllCoroutines();
            scrollView.GetComponent<ScrollRect>().StopMovement();
            onClose?.Invoke();
        });
        StartCoroutine(Move(prev, cur));
    }
    
    private IEnumerator Move(int prev, int cur)
    {
        yield return null;
        scrollView.refresh();
        MoveScrollImmediately(prev);
        StartCoroutine(MoveScrollItem(cur));
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
        scrollRect.content.transform.localPosition = new Vector3(0f, targetY+(scrollRect.viewport.rect.height/2), 0f);
    }
    
    private IEnumerator MoveScrollItem(int index)
    {
        yield return new WaitForSeconds(1.0f);
        var scrollRect = scrollView.GetComponent<ScrollRect>();
        var itemHeight = scrollView.itemPrototype.rect.height;
        var viewportHeight = scrollRect.viewport.rect.height;

        var targetY = itemHeight * (index + 0.5f) - viewportHeight / 2f;

        var minY = 0f;
        var maxY = scrollView.totalItemCount * itemHeight - viewportHeight;
        targetY = Mathf.Clamp(targetY, minY, maxY);
        scrollRect.content.transform.DOLocalMove(new Vector3(0f, targetY+(scrollRect.viewport.rect.height/2), 0f), 1f).SetEase(Ease.OutExpo).OnComplete(
            () =>
            {
                closeButton.gameObject.SetActive(true);
            });
    }

}
