using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIColorMatchRetryPopup : UIPopupPanel
{
    public Button exitButton;
    public Button retryButton;
    
    public TextMeshProUGUI curScoreText;
    public TextMeshProUGUI maxScoreText;
    
    private Action _exitEvent;
    private Action _retryEvent;
    
    public void Set(int curScore, int maxScore)
    {
        exitButton.onClick.RemoveAllListeners();
        exitButton.onClick.AddListener(Exit);
        retryButton.onClick.RemoveAllListeners();
        retryButton.onClick.AddListener(Retry);
        
        curScoreText.text = $"CurScore: {curScore}";
        maxScoreText.text = $"MaxScore: {maxScore}";
    }

    public void AddExitEvent(Action onExit)
    {
        this._exitEvent = onExit;
    }

    public void AddRetryEvent(Action onRetry)
    {
        this._retryEvent = onRetry;
    }

    
    private void Exit()
    {
        _exitEvent?.Invoke();
        _exitEvent = null;
        this.gameObject.SetActive(false);
        Main.Ins.MainGame.ReturnToLobby();
    }
    
    private void Retry()
    {
        _retryEvent?.Invoke();
        _retryEvent = null;
        Main.Ins.MainGame.RetryGame();
        gameObject.SetActive(false);
    }
}
