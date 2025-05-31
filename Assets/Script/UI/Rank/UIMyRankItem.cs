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
    public Image medalImage;
    public Sprite[] medalSprites;
    private int _index;

    public void Set()
    {
        nameText.text = Main.Ins.MainData.UserData.UserInfo.Name;
        scoreText.text = Main.Ins.MainData.UserData.UserInfo.Score.ToString();
        var rank = Main.Ins.MainData.RankData.GetRankNumber(Login.Ins.UserID);
        rankText.text = rank == -1 ? "-" : rank.ToString();
    }
}
