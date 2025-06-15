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
    public UIColorMatch UIColorMatch { get; private set; } 

    public bool LoadComplete { get; private set; }
    public IObservable<Unit> OnLoadComplete => _onLoadComplete;
    private Subject<Unit> _onLoadComplete = new Subject<Unit>();
    private void Awake()
    {
        UiPopup = FindObjectOfType<UIPopup>();
        UiLobby = FindObjectOfType<UILobby>();
        UIColorMatch = FindObjectOfType<UIColorMatch>();
        UIColorMatch.gameObject.SetActive(false);
        Initialize();
    }

    private void Initialize()
    {
        if (Main.Ins.LoadComplete)
        {
            UiPopup.Initialize();
            UiLobby.Initialize();   
            
            LoadComplete = true;
            _onLoadComplete.OnNext(Unit.Default);
            _onLoadComplete.OnCompleted();
            _onLoadComplete.Dispose();
            _onLoadComplete = null;
        }
        else
        {
            Main.Ins.OnLoadComplete.Subscribe((_) =>
            {
                UiPopup.Initialize();
                UiLobby.Initialize();   
                
                LoadComplete = true;
                _onLoadComplete.OnNext(Unit.Default);
                _onLoadComplete.OnCompleted();
                _onLoadComplete.Dispose();
                _onLoadComplete = null;
            }).AddTo(this);
        }
        
    }
}
