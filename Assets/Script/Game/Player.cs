using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UniRx;
using UnityEngine;

public enum PlayerMove
{
    Left,
    Right,
    Back,
    Forward,
}

public class Player : MonoBehaviour
{
    public PlayerController playerController;
    public Animator animator;
    
    public void Initialized(Vector3 pos)
    {
        transform.position = new Vector3(pos.x, pos.y+(transform.localScale.y/2.0f), pos.z);
    }
    public void Move(PlayerMove move, Vector3 pos)
    {
        var movePos = new Vector3(pos.x, pos.y+(transform.localScale.y/2.0f), pos.z);
        
        if (move == PlayerMove.Left)
        {
            MoveLeft(movePos);
        }
        else if (move == PlayerMove.Right)
        {
            MoveRight(movePos);
        }
        else if (move == PlayerMove.Back)
        {
            MoveBack();
        }
        else if (move == PlayerMove.Forward)
        {
            MoveForward(movePos);
        }
    }
    
    public float _jumpHeight = 0.2f;
    public float _moveDuration = 0.2f;
    public float _lerp = 0.5f;
    
    public void MoveLeft(Vector3 movePos)
    {
        transform.rotation = Quaternion.LookRotation(Vector3.left);
        animator.SetTrigger("Attack");

        Vector3 startPos = transform.position;
        Vector3 midPos = Vector3.Lerp(startPos, movePos, _lerp);
        midPos.y += _jumpHeight;

        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOMove(midPos, _moveDuration / 2f).SetEase(Ease.OutQuad));
        seq.Append(transform.DOMove(movePos, _moveDuration / 2f).SetEase(Ease.InQuad));
    }

    public void MoveRight(Vector3 movePos)
    {
        transform.rotation = Quaternion.LookRotation(Vector3.right);
        animator.SetTrigger("Attack");

        Vector3 startPos = transform.position;
        Vector3 midPos = Vector3.Lerp(startPos, movePos, _lerp);
        midPos.y += _jumpHeight;

        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOMove(midPos, _moveDuration / 2f).SetEase(Ease.OutQuad));
        seq.Append(transform.DOMove(movePos, _moveDuration / 2f).SetEase(Ease.InQuad));
    }

    public void MoveBack()
    {
        transform.rotation = Quaternion.LookRotation(Vector3.back);
    }

    public void MoveForward(Vector3 movePos)
    {
        transform.rotation = Quaternion.LookRotation(Vector3.forward);
        animator.SetTrigger("Attack");
        Vector3 startPos = transform.position;
        Vector3 midPos = Vector3.Lerp(startPos, movePos, _lerp);
        midPos.y += _jumpHeight;
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOMove(midPos, _moveDuration / 2f).SetEase(Ease.OutQuad));
        seq.Append(transform.DOMove(movePos, _moveDuration / 2f).SetEase(Ease.InQuad));
    }
}