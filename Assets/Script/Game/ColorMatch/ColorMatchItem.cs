using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ColorMatchItem : MonoBehaviour, IPointerClickHandler
{
    public Image idleImage;
    public Image successImage;
    public Image failImage;

    private int _index;
    
    public void SetIndex(int index)
    {
        _index = index;
    }
    
    public void SetItem(Color color)
    {
        idleImage.color = color;
        successImage.color = color;
        failImage.color = color;
        
        idleImage.gameObject.SetActive(true);
        successImage.gameObject.SetActive(false);
        failImage.gameObject.SetActive(false);
    }

    public void ShowSuccess()
    {
        successImage.gameObject.SetActive(true);
        idleImage.gameObject.SetActive(false);
        failImage.gameObject.SetActive(false);
    }

    public void ShowFail()
    {
        failImage.gameObject.SetActive(true);
        successImage.gameObject.SetActive(false);
        idleImage.gameObject.SetActive(false);
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        var game = Main.Ins.MainGame.GameContentProvider.GetGameContent<ColorMatchContent>(GameType.ColorMatch);
        game.Select(_index);
    }
}
