using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIColorMatchButton : MonoBehaviour
{
    public Button button;

    private void Awake()
    {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => Main.Ins.MainGame.EnterGame(GameType.ColorMatch));
    }
}
