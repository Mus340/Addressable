using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIReturnToLobbyPopup : UIPopupPanel
{
    public Button closeButton;
    public Button returnButton;


    private void Start()
    {
        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(Close);
        returnButton.onClick.RemoveAllListeners();
        returnButton.onClick.AddListener(ReturnToLobby);
    }

    private void Close()
    {
        this.gameObject.SetActive(false);
    }
    
    private void ReturnToLobby()
    {
        Main.Ins.ReturnToLobby();
        gameObject.SetActive(false);
    }
}
