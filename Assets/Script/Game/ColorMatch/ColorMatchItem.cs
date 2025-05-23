using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ColorMatchItem : MonoBehaviour, IPointerClickHandler
{
    public Image itemImage;
    public Image successImage;
    public Image failImage;

    private int _index;

    public void SetIndex(int index)
    {
        _index = index;
    }
    
    public void SetItem(Color color)
    {
        itemImage.color = color;
        successImage.gameObject.SetActive(false);
        failImage.gameObject.SetActive(false);
    }

    public void SetSuccess()
    {
        successImage.gameObject.SetActive(true);
        failImage.gameObject.SetActive(false);
    }

    public void SetFail()
    {
        successImage.gameObject.SetActive(true);
        failImage.gameObject.SetActive(false);
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        var game = Main.Ins.MainGame.GameContentProvider.GetGameContent<ColorMatchContent>(GameType.ColorMatch);
        game.Select(_index);
    }
}
