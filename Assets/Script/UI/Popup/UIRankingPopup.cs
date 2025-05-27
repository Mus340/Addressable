using System;
using System.Collections;
using System.Collections.Generic;
using Mosframe;
using UniRx;
using UnityEngine;

public class UIRankingPopup : UIPopupPanel
{
    public UIMyRankItem MyRankItem;
    public DynamicVScrollView ScrollView;

    private void Awake()
    {
        if (Main.Ins.LoadComplete)
        {
            ScrollView.totalItemCount = Main.Ins.MainData.UserRankList.Count;
        }
        else
        {
            Main.Ins.OnLoadComplete.Subscribe((_) =>
            {
                ScrollView.totalItemCount = Main.Ins.MainData.UserRankList.Count;
            }).AddTo(this);
        }
    }

    public void Set()
    {
        MyRankItem.Set();
        ScrollView.refresh();
    }
}
