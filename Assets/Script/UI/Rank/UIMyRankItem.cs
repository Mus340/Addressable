using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIMyRankItem : MonoBehaviour
{
    public TextMeshProUGUI rankText;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI playCountText;
    public TextMeshProUGUI scoreText;
    public Image medalImage;
    public Sprite[] medalSprites;
    public GameObject lockPanel;
    private int _index;
    
    public void Set()
    {
        nameText.text = Main.Ins.MainUser.GetName();
        
        var score = Main.Ins.MainUser.GetScore();
        if (score <= 0)
        {
            lockPanel.gameObject.SetActive(true);
        }
        else
        {
            scoreText.text = Main.Ins.MainUser.GetScore().ToString();
            playCountText.text = Main.Ins.MainUser.GetPlayCount().ToString();
            lockPanel.gameObject.SetActive(false);
        }
        var rank = Main.Ins.MainUser.GetRank();
        rankText.text = rank == -1 ? "-" : rank.ToString();
    }
}
