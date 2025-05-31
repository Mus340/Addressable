using System.Collections;
using System.Collections.Generic;
using TMPro;
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

        goldRankerText.text = Main.Ins.MainData.RankData.RankList[0].Name;
        silverRankerText.text = Main.Ins.MainData.RankData.RankList[1].Name;
        bronzeRankerText.text = Main.Ins.MainData.RankData.RankList[2].Name;
    }

    private void Enter()
    {
        Main.Ins.MainGame.EnterGame(GameType.ColorMatch);
    }
}
