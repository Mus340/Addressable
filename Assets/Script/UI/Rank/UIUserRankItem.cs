using System.Collections;
using System.Collections.Generic;
using Mosframe;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIUserRankItem : MonoBehaviour, IDynamicScrollViewItem
{
    public Text rankText;
    public Text nameText;
    public Text scoreText;
    public Image medalImage;
    public Image bgImage;
    public Sprite[] medalSprites;
    
    private int _index;
    
    public void onUpdateItem(int index)
    {
        _index = index;
        var rankData = Main.Ins.MainData.RankData.GetRank(index);
        nameText.text = rankData.Name;
        scoreText.text = rankData.MaxScore.ToString();
        rankText.text = (_index+1).ToString();

        if (index == 0)
        {
            medalImage.sprite = medalSprites[0];
        }
        else if (index == 1)
        {
            medalImage.sprite = medalSprites[1];
        }
        else if (index == 2)
        {
            medalImage.sprite = medalSprites[2];
        }
        else
        {
            medalImage.sprite = medalSprites[3];
        }

        if (index == Main.Ins.MainData.RankData.MyRankIndex)
        {
            bgImage.color = new Color(0.5843138f, 0.2588235f, 1);
        }
        else
        {
            bgImage.color = new Color(0.7176471f, 0.4980392f, 1);
        }
    }
}
