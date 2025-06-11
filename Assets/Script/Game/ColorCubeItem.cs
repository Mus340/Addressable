using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UniRx;
using UnityEngine;

public class ColorCubeItem : MonoBehaviour
{
    private Color _originalColor;
    private int _level;
    private int _index;
    private CompositeDisposable _disposable;
    
    public void OnEnable()
    {
        _disposable = new CompositeDisposable();
        var content = Main.Ins.MainGame.GetGame<ColorMatchContent>();
        content.OnFail.Subscribe((_) =>
        {
            if (_level == content.Level+1 && _index == content.AnswerList[_level])
            {
                ShowAnswer(1.0f);
            }
        }).AddTo(_disposable);
    }

    private void OnDisable()
    {
        _disposable?.Dispose();
        _disposable = null;
    }

    public void SetData(int level, int index, Color color)
    {
        _level = level;
        _index = index;
        _originalColor = color;
        GetComponent<Renderer>().material.color = color;
    }

    private void ShowAnswer(float duration)
    {
        StopAllCoroutines();
        StartCoroutine(FlashColor(duration));
    }

    private IEnumerator FlashColor(float duration)
    {
        var render = GetComponent<Renderer>();
        var mat = render.material;
        var elapsed = 0f;
        var interval = 1.0f;
        var half = interval / 2f;

        while (elapsed < duration)
        {
            yield return mat.DOColor(Color.white, half).WaitForCompletion();
            elapsed += half;
            if (elapsed >= duration) break;
            yield return mat.DOColor(_originalColor, half).WaitForCompletion();
            elapsed += half;
        }
        mat.color = _originalColor;
    }

}

