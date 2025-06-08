using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Vector2 startPos;
    private bool isSwiping = false;
    public float minSwipeDistance = 50f; // 최소 스와이프 판정 거리 (픽셀)

    public IObservable<PlayerMove> OnMove => _onMove;
    private ISubject<PlayerMove> _onMove = new Subject<PlayerMove>();
    
    void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        // 마우스 (에디터 테스트용)
        if (Input.GetMouseButtonDown(0))
        {
            isSwiping = true;
            startPos = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0) && isSwiping)
        {
            Vector2 endPos = Input.mousePosition;
            DetectSwipe(startPos, endPos);
            isSwiping = false;
        }
#else
        // 터치 (모바일)
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                isSwiping = true;
                startPos = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended && isSwiping)
            {
                Vector2 endPos = touch.position;
                DetectSwipe(startPos, endPos);
                isSwiping = false;
            }
        }
#endif
    }

    void DetectSwipe(Vector2 start, Vector2 end)
    {
        Vector2 delta = end - start;

        if (delta.magnitude < minSwipeDistance)
        {
            return;
        }

        if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
        {
            if (delta.x > 0)
            {
                _onMove.OnNext(PlayerMove.Right);
                Debug.Log("Swipe Right");
            }
            else
            {
                _onMove.OnNext(PlayerMove.Left);
                Debug.Log("Swipe Left");
            }
        }
        else
        {
            if (delta.y > 0)
            {
                _onMove.OnNext(PlayerMove.Forward);
                Debug.Log("Swipe Up");
            }
            else
            {
                _onMove.OnNext(PlayerMove.Back);
                Debug.Log("Swipe Down");
            }
        }
    }
}
