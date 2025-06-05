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
    private int _index;

    public void Set()
    {
        var userName = Main.Ins.MainData.UserData.UserInfo.Name;
        nameText.text = userName;
        scoreText.text = Main.Ins.MainData.UserData.UserInfo.Score.ToString();
        //var rank = Main.Ins.MainData.RankData.GetRankNumber(userName);
        //rankText.text = rank == -1 ? "-" : rank.ToString();
    }
}
