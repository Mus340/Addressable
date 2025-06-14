using System;
using System.Collections;
using System.Collections.Generic;
using Mosframe;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class UIRankingPopup : UIPopupPanel
{
    [Serializable]
    public class RankingView
    {
        public Button Button;
        public Text Text;
    }
    public UIMyRankItem MyRankItem;
    public RankingView[] RankingViews;
    public DynamicVScrollView ScrollView;
    public RectTransform ScrollViewRect;
    public Tier? SelectTier { get; private set; }

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
        for (int i = 0; i < RankingViews.Length; i++)
        {
            RankingViews[i].Button.onClick.RemoveAllListeners();
            var tier = (Tier)i;
            RankingViews[i].Button.onClick.AddListener(()=>
            {
                OpenView(tier);
            });
        }
    }

    public void Open()
    {
        if (SelectTier.HasValue)
        {
            OpenView(SelectTier.Value);
        }
        else
        {
            SelectTier = Main.Ins.MainData.RankData.GetTier();
            OpenView(SelectTier.Value);
        }
        MyRankItem.Set();
    }

    private void OpenView(Tier tier)
    {
        RankingViews[(int)SelectTier].Text.color = Color.white;
        SelectTier = tier;
        RankingViews[(int)SelectTier].Text.color = Color.yellow;
        ScrollView.totalItemCount = Main.Ins.MainData.RankData.GetRankList(tier).Count;
        ScrollView.refresh();
    }
}
