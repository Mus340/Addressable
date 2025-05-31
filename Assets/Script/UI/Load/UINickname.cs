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
        confirmButton.onClick.AddListener(() =>
        {
            StartCoroutine(ConfirmAfterInputSettled());
        });

        inputField.onEndEdit.RemoveAllListeners();
        inputField.onEndEdit.AddListener((_) =>
        {
            StartCoroutine(ConfirmAfterInputSettled());
        });
    }

    private IEnumerator ConfirmAfterInputSettled()
    {
        yield return new WaitForEndOfFrame();

        var nickname = inputField.text.Trim();
        if (!string.IsNullOrEmpty(nickname))
        {
            nicknameSetter.SetNickName(nickname);
        }
        else
        {
            Debug.LogWarning("닉네임이 비어 있습니다.");
        }
    }
}
