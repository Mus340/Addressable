using System;
using System.Collections;
using System.Collections.Generic;
using Mosframe;
using UniRx;
using UnityEngine;

public class UIRankingPopup : UIPopupPanel
{
    public DynamicVScrollView ScrollView;

    private void Awake()
    {
        if (Main.Ins.LoadComplete)
        {
            ScrollView.totalItemCount = Main.Ins.MainRank.RankList.Count;
        }
        else
        {
            Main.Ins.OnLoadComplete.Subscribe((_) =>
            {
                ScrollView.totalItemCount = Main.Ins.MainRank.RankList.Count;
            }).AddTo(this);
        }
    }
}
