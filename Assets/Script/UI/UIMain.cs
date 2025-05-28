using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Rendering;

public class UIMain : MonoBehaviour
{
    private static UIMain _ins;   
    public static UIMain Ins
    {
        get
        {
            if (_ins == null)
            {
                _ins = FindObjectOfType<UIMain>();
            }
            return _ins;
        }
    }

    public UIPopup UiPopup {get; private set;}
    public UILobby UiLobby {get; private set;}

    private void Awake()
    {
        UiPopup = FindObjectOfType<UIPopup>();
        UiLobby = FindObjectOfType<UILobby>();
        Initialize();
    }

    private void Initialize()
    {
        if (Main.Ins.LoadComplete)
        {
            UiPopup.Initialize();
            UiLobby.Initialize();   
        }
        else
        {
            Main.Ins.OnLoadComplete.Subscribe((_) =>
            {
                UiPopup.Initialize();
                UiLobby.Initialize();   
            }).AddTo(this);
        }
    }
}
