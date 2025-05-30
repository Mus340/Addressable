using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UINickname : MonoBehaviour
{
    public NicknameSetter nicknameSetter;
    public TMP_InputField inputField;
    public Button confirmButton;

    private void Awake()
    {
        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(Confirm);
    }

    private void Confirm()
    {
        var nickname = inputField.text;
        nicknameSetter.SetNickName(nickname);
    }
}
