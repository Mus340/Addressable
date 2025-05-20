using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIReturnToLobbyButton : MonoBehaviour
{
    private Button _button;
    
    public void Start()
    {
        _button = this.GetComponent<Button>();
        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(OpenPopup);
    }
    
    private void OpenPopup()
    {
        var popup = UIMain.Ins.UiPopup.GetPopup(PopupType.ReturnToLobby);
        popup.gameObject.SetActive(true);
    }
}
