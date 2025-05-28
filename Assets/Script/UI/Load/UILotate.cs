using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILotate : MonoBehaviour
{
    public float rotateSpeed;
    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        StartCoroutine(Rotate());
    }

    private IEnumerator Rotate()
    {
        while (true)
        {
            rectTransform.Rotate(0f, 0f, -(rotateSpeed * Time.deltaTime));
            yield return null;
        }
    }
}
