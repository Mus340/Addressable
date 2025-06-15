using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIChangeTierPopup : UIPopupPanel
{
    public Text titleText;
    public Image prevTierImage;
    public Image curTierImage;
    public Button confirmButton;
    public Text rankText;

    public void Set(Tier prev, Tier cur, Action onClose)
    {
        titleText.text = prev >= cur ? "강등" : "승급";
        prevTierImage.sprite = Main.Ins.MainData.RankData.tierSprite[(int) prev];
        curTierImage.sprite = Main.Ins.MainData.RankData.tierSprite[(int) cur];
        rankText.text = $"{Main.Ins.MainData.RankData.GetRankNumber()+1}등";
        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(()=>
        {
            onClose?.Invoke();
            gameObject.SetActive(false);
        });
    }
}
