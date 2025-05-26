using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILobby : MonoBehaviour
{
    public Button enterButton;
    
    public void Initialize()
    {
        enterButton.onClick.RemoveAllListeners();
        enterButton.onClick.AddListener(Enter);
    }

    private void Enter()
    {
        Main.Ins.MainGame.EnterGame(GameType.ColorMatch);
    }
}
