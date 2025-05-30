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
        nameText.text = Main.Ins.MainUser.GetName();
        scoreText.text = Main.Ins.MainUser.GetScore().ToString();
        var rank = Main.Ins.MainUser.GetRank();
        rankText.text = rank == -1 ? "-" : rank.ToString();
    }
}
