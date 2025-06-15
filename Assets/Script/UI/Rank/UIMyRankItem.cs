using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIMyRankItem : MonoBehaviour
{
    public Text rankText;
    public Text nameText;
    public Text scoreText;
    public Image tierImage;

    public void Set()
    {
        var userName = Main.Ins.MainData.UserData.UserInfo.Name;
        nameText.text = userName;
        scoreText.text = Main.Ins.MainData.UserData.UserInfo.Score.ToString();
        var rank = Main.Ins.MainData.RankData.GetRankNumber();
        rankText.text = (rank+1).ToString();
        var tier = Main.Ins.MainData.RankData.GetTier(Main.Ins.MainData.UserData.UserInfo.Score);
        tierImage.sprite = Main.Ins.MainData.RankData.tierSprite[(int)tier];
    }
}
