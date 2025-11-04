using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIMyRankItem : MonoBehaviour
{
    public Text rankText;
    public Text nameText;
    public Text scoreText;
    public Image rankImage;
    public Sprite[] rankSprites;
    public Sprite defaultSprite;
    
    public void Set()
    {
        var userName = Main.Ins.MainData.UserData.UserInfo.Name;
        nameText.text = userName;
        var rank = Main.Ins.MainData.RankData.MyRankIndex;
        if (rank == -1)
        {
            rankImage.sprite = defaultSprite;
            rankText.text = "미참여";
            scoreText.text = "-";
        }
        else
        {
            rankImage.sprite = rank < 3 ? rankSprites[rank] : defaultSprite;
            rankText.text = (rank+1).ToString();
            scoreText.text = Main.Ins.MainData.UserData.UserInfo.Score.ToString();
        }
    }
}
