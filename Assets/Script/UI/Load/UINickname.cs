using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UINickname : MonoBehaviour
{
    public NicknameSetter nicknameSetter;
    public TMP_InputField inputField;
    public Button confirmButton;
    
    public GameObject warningPanel;
    public Text warningText;
    private CanvasGroup _warningCanvasGroup;
    private Coroutine _closeCoroutine;
    private Tween _fadeTween;

    private void Awake()
    {
        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(() =>
        {
            var nickname = inputField.text.Trim();
            inputField.text = nickname;
            nicknameSetter.SetNickName(nickname, (resultStr, resultType) =>
            {
                if (resultType == false)
                {
                    OpenWarning(resultStr);
                }
            });
        });

        inputField.onEndEdit.RemoveAllListeners();
        inputField.onEndEdit.AddListener((_) =>
        {
            StartCoroutine(ConfirmAfterInputSettled());
        });
        
        _warningCanvasGroup = warningPanel.GetComponent<CanvasGroup>();
        _warningCanvasGroup.alpha = 0f;
        
        warningPanel.SetActive(false);
    }

    private IEnumerator ConfirmAfterInputSettled()
    {
        yield return null;
        var nickname = inputField.text.Trim();
        inputField.text = nickname;
    }

    private void OpenWarning(string warning)
    {
        warningText.text = warning;
        warningPanel.SetActive(true);
        _warningCanvasGroup.alpha = 1f;

        if (_closeCoroutine != null)
        {
            StopCoroutine(_closeCoroutine);
            _fadeTween?.Kill();
        }
        _closeCoroutine = StartCoroutine(CloseFadeWarning());
    }
    
    private IEnumerator CloseFadeWarning()
    {
        yield return new WaitForSeconds(1.5f);
        _fadeTween = _warningCanvasGroup.DOFade(0f, 3.0f).SetEase(Ease.InOutSine);
        yield return new WaitForSeconds(1.5f);
        warningPanel.SetActive(false);
    }
}
