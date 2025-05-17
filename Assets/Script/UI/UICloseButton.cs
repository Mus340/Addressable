using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICloseButton : MonoBehaviour
{
    public Button myButton;
    
    private void Start()
    {
        myButton.onClick.RemoveAllListeners();
    }

    public void Close(Action onClose)
    {
        onClose?.Invoke();
    }
}
