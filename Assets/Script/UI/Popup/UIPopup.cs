using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PopupType
{
    Setting,
    DefaultInfo,
    ColorMatchInfo,
    ReturnToLobby,
}
public class UIPopup : MonoBehaviour
{
    private Dictionary<PopupType, UIPopupPanel> _panels;

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        _panels = new Dictionary<PopupType, UIPopupPanel>();
        foreach (PopupType popupType in Enum.GetValues(typeof(PopupType)))
        {
            var prefab = Resources.Load<UIPopupPanel>($"{ResourcesPath.PopupPath}{popupType}");
            var popup = Instantiate(prefab, this.transform);
            popup.gameObject.SetActive(false);
            _panels.Add(popupType, popup);
        }
    }
    
    public UIPopupPanel GetPopup(PopupType type)
    {
        if (_panels.TryGetValue(type, out var panel))
        {
            return panel;
        }
        return null;
    }

    public T GetPopup<T>(PopupType type) where T : UIPopupPanel
    {
        if (_panels.TryGetValue(type, out var panel))
        {
            return panel as T;
        }
        return null;
    }
}
