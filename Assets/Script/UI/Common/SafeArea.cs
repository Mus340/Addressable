using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class SafeArea : MonoBehaviour
{
    private RectTransform _rectTransform;
    private Rect _lastSafeArea;
    private ScreenOrientation _lastOrientation;
    private Vector2Int _lastResolution;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        ApplySafeArea();
    }

    private void Update()
    {
        if (HasScreenChanged())
        {
            ApplySafeArea();
            Debug.Log("CHange");
        }
    }

    private bool HasScreenChanged()
    {
        return
            _lastSafeArea != Screen.safeArea ||
            _lastOrientation != Screen.orientation ||
            _lastResolution.x != Screen.width ||
            _lastResolution.y != Screen.height;
    }

    private void ApplySafeArea()
    {
        Rect safeArea = Screen.safeArea;

        Vector2 anchorMin = safeArea.position;
        Vector2 anchorMax = safeArea.position + safeArea.size;

        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        _rectTransform.anchorMin = anchorMin;
        _rectTransform.anchorMax = anchorMax;

        _lastSafeArea = safeArea;
        _lastOrientation = Screen.orientation;
        _lastResolution = new Vector2Int(Screen.width, Screen.height);
    }
}
