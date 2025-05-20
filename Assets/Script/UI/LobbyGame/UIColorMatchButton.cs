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
        button.onClick.AddListener(OpenInfoPopup);
    }

    private void OpenInfoPopup()
    {
        var popup = UIMain.Ins.UiPopup.GetPopup<UIDefaultInfoPopup>(PopupType.DefaultInfo);
        popup.SetData(GameType.ColorMatch);
        popup.gameObject.SetActive(true);
    }
}
