using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIReturnToLobbyPopup : UIPopupPanel
{
    public Button closeButton;
    public Button returnButton;
    
    private Action _closeEvent;
    private Action _returnEvent;
    
    private void Start()
    {
        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(Close);
        returnButton.onClick.RemoveAllListeners();
        returnButton.onClick.AddListener(ReturnToLobby);
    }

    public void AddCloseEvent(Action onClose)
    {
        this._closeEvent = onClose;
    }

    public void AddReturnToLobbyEvent(Action onReturn)
    {
        this._returnEvent = onReturn;
    }

    
    private void Close()
    {
        _closeEvent?.Invoke();
        _closeEvent = null;
        this.gameObject.SetActive(false);
    }
    
    private void ReturnToLobby()
    {
        _returnEvent?.Invoke();
        _returnEvent = null;
        Main.Ins.MainGame.ReturnToLobby();
        gameObject.SetActive(false);
    }
}
