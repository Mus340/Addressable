using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UISettingPopup : UIPopupPanel
{
    public Button languageButton;
    
    public Button muteButton;
    public Image muteImage;
    public Sprite[] muteSprites;
    public Sprite[] buttonSprites;
    
    private void Awake()
    {
        languageButton.onClick.RemoveAllListeners();
        muteButton.onClick.RemoveAllListeners();
        languageButton.onClick.AddListener(Language);
        muteButton.onClick.AddListener(Mute);
    }

    public void Open()
    {
        var state = PlayerPrefs.GetInt("Mute", 0);
        muteImage.sprite = muteSprites[state];
        muteButton.image.sprite = buttonSprites[state];
    }
    
    private void Language()
    {
        
    }
    
    private void Mute()
    {
        var state = PlayerPrefs.GetInt("Mute", 0);
        state = 1 - state;
        PlayerPrefs.SetInt("Mute", state);
        PlayerPrefs.Save();
        muteImage.sprite = muteSprites[state];
        muteButton.image.sprite = buttonSprites[state];
    }
}
