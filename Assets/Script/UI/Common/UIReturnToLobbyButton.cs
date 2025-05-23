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
        var popup = UIMain.Ins.UiPopup.GetPopup<UIReturnToLobbyPopup>(PopupType.ReturnToLobby);
        Main.Ins.MainTime.Pause();
        popup.AddCloseEvent(Main.Ins.MainTime.Resume);
        popup.AddReturnToLobbyEvent(Main.Ins.MainTime.Resume);
        popup.gameObject.SetActive(true);
    }
}
