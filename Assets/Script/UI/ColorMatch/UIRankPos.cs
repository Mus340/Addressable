using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRankPos : MonoBehaviour
{
    public Image image;
    public Text text;
    
    private int _rank;
    private bool _isDeath;
    
    public void Initialize(int rank)
    {
        _isDeath = true;
        _rank = rank;
        text.text = (_rank + 1).ToString();
        Color color;
        if (_rank == 0)
        {
            ColorUtility.TryParseHtmlString("#FFFF00", out color);
        }
        else if (_rank == 1)
        {
            ColorUtility.TryParseHtmlString("#ABABAB", out color);
        }
        else if (_rank == 2)
        {
            ColorUtility.TryParseHtmlString("#FF6700", out color);
        }
        else
        {
            ColorUtility.TryParseHtmlString("#FFFFFF", out color);
        }
        image.color = color;
    }

    public void Death()
    {
        
    }
}
