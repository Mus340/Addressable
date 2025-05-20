using System;
using System.Collections;
using System.Collections.Generic;
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
    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        UiPopup = FindObjectOfType<UIPopup>();
    }
}
