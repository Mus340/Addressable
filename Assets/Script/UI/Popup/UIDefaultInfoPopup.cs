using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIDefaultInfoPopup : UIPopupPanel
{
    public Button enterButton;
    public TextMeshProUGUI titleText;
    
    private GameType _gameType;
    
    public void SetData(GameType gameType)
    {
        this._gameType = gameType;
        
        enterButton.onClick.RemoveAllListeners();
        enterButton.onClick.AddListener(EnterGame);
        titleText.text = _gameType.ToString();
    }
    
    private void EnterGame()
    {
        gameObject.SetActive(false);
        Main.Ins.MainGame.EnterGame(_gameType);
    }

}
