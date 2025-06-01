using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISettingTop : MonoBehaviour
{
    private Button _button;

    private void Awake()
    {
        this._button = GetComponent<Button>();
        this._button.onClick.RemoveAllListeners();
        this._button.onClick.AddListener(Open);
    }

    private void Open()
    {
        var popup = UIMain.Ins.UiPopup.GetPopup<UISettingPopup>(PopupType.Setting);
        popup.Open();
        popup.gameObject.SetActive(true);
    }
}
