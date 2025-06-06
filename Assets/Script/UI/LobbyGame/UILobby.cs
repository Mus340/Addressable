using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class UILobby : MonoBehaviour
{
    public Button enterButton;
    
    public Text goldRankerText;
    public Text silverRankerText;
    public Text bronzeRankerText;
    
    public void Initialize()
    {        
        enterButton.onClick.RemoveAllListeners();
        enterButton.onClick.AddListener(Enter);

        goldRankerText.text = "변경중";
        silverRankerText.text = "변경중";
        bronzeRankerText.text = "변경중";
    }

    private void Enter()
    {
        Main.Ins.MainGame.EnterGame();
    }
}
