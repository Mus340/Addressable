using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UILobby : MonoBehaviour
{
    public Button enterButton;
    
    public TextMeshProUGUI goldRankerText;
    public TextMeshProUGUI silverRankerText;
    public TextMeshProUGUI bronzeRankerText;
    
    public void Initialize()
    {
        enterButton.onClick.RemoveAllListeners();
        enterButton.onClick.AddListener(Enter);

        goldRankerText.text = Main.Ins.MainData.UserRankList[0].Name;
        silverRankerText.text = Main.Ins.MainData.UserRankList[1].Name;
        bronzeRankerText.text = Main.Ins.MainData.UserRankList[2].Name;
    }

    private void Enter()
    {
        Main.Ins.MainGame.EnterGame(GameType.ColorMatch);
    }
}
