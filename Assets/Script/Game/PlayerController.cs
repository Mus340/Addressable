using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public enum PlayerMove
{
    Left,
    Right,
    Back,
    Forward,
}
public class PlayerController : MonoBehaviour
{
    private Vector2 _startPos;
    private bool _isSwiping = false;
    private float _minSwipeDistance = 50f;

    public IObservable<PlayerMove> OnMove => _onMove;
    private ISubject<PlayerMove> _onMove = new Subject<PlayerMove>();
    
    private void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButtonDown(0))
        {
            _isSwiping = true;
            _startPos = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0) && _isSwiping)
        {
            Vector2 endPos = Input.mousePosition;
            DetectSwipe(_startPos, endPos);
            _isSwiping = false;
        }
#else
        // 터치 (모바일)
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                _isSwiping = true;
                _startPos = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended && _isSwiping)
            {
                Vector2 endPos = touch.position;
                DetectSwipe(_startPos, endPos);
                _isSwiping = false;
            }
        }
#endif
    }

    private void DetectSwipe(Vector2 start, Vector2 end)
    {
        Vector2 delta = end - start;

        if (delta.magnitude < _minSwipeDistance)
        {
            return;
        }

        if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
        {
            if (delta.x > 0)
            {
                _onMove.OnNext(PlayerMove.Right);
            }
            else
            {
                _onMove.OnNext(PlayerMove.Left);
            }
        }
        else
        {
            if (delta.y > 0)
            {
                _onMove.OnNext(PlayerMove.Forward);
            }
            else
            {
                _onMove.OnNext(PlayerMove.Back);
            }
        }
    }
}
