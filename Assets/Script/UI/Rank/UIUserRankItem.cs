using System.Collections;
using System.Collections.Generic;
using Mosframe;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIUserRankItem : MonoBehaviour, IDynamicScrollViewItem
{
    public TextMeshProUGUI rankText;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI scoreText;
    public Image medalImage;

    public Sprite[] medalSprites;
    
    private int _index;
    
    public void onUpdateItem(int index)
    {
        _index = index;
        nameText.text = Main.Ins.MainRank.RankList[index].UserName;
        scoreText.text = Main.Ins.MainRank.RankList[index].Score.ToString();
        rankText.text = _index.ToString();

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
    }
}
