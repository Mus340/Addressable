using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;


public class Player : MonoBehaviour
{
    public IObservable<Vector3Int> OnMove => _onMove;
    private ISubject<Vector3Int> _onMove = new Subject<Vector3Int>();
    
    public PlayerController playerController;
    public Animator animator;
    public float jumpHeight;
    public float moveDuration;
    public float lerp;

    private ColorMatchContent _content;
    private Vector3Int _pos;

    public Vector3Int GetPos() => _pos;
    
    public void Initialized(Vector3 pos)
    {
        transform.position = new Vector3(pos.x, pos.y+(transform.localScale.y/2.0f), pos.z);
        _content = Main.Ins.MainGame.GetGame<ColorMatchContent>();
        
        playerController.OnMove.Subscribe((move) =>
        {
            if (move == PlayerMove.Left)
            {
                Check(PlayerMove.Left);
            }
            else if (move == PlayerMove.Right)
            {
                Check(PlayerMove.Right);
            }
            else if (move == PlayerMove.Back)
            {
                Check(PlayerMove.Back);
            }
            else if (move == PlayerMove.Forward)
            {
                _content.Select(_pos.x);
            }
        }).AddTo(this);
        
        _content.OnNext.Subscribe((level) =>
        {
            _pos.y = level;
            _pos.z = level;
            Move(PlayerMove.Forward,new Vector3(_pos.x, _pos.y, _pos.z));
            _onMove.OnNext(new Vector3Int(_pos.x, _pos.y, _pos.z));
        }).AddTo(this);
    }
    
    private void Check(PlayerMove move)
    {
        if (!_content.IsEndGame)
        {
            if (move == PlayerMove.Left && (_pos.x-1) >= 0)
            {
                _pos.x--;
            }
            else if (move == PlayerMove.Right && (_pos.x + 1) < _content.LevelData.GetValue(_pos.y).cube_count)
            {
                _pos.x++;
            }
            Move(move, new Vector3(_pos.x, _pos.y, _pos.z));
            _onMove.OnNext(new Vector3Int(_pos.x, _pos.y, _pos.z));
        }
    }
    
    private void Move(PlayerMove move, Vector3 pos)
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
    
    private void MoveLeft(Vector3 movePos)
    {
        transform.rotation = Quaternion.LookRotation(Vector3.left);
        animator.SetTrigger("Attack");

        Vector3 startPos = transform.position;
        Vector3 midPos = Vector3.Lerp(startPos, movePos, lerp);
        midPos.y += jumpHeight;

        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOMove(midPos, moveDuration / 2f).SetEase(Ease.OutQuad));
        seq.Append(transform.DOMove(movePos, moveDuration / 2f).SetEase(Ease.InQuad));
    }

    private void MoveRight(Vector3 movePos)
    {
        transform.rotation = Quaternion.LookRotation(Vector3.right);
        animator.SetTrigger("Attack");

        Vector3 startPos = transform.position;
        Vector3 midPos = Vector3.Lerp(startPos, movePos, lerp);
        midPos.y += jumpHeight;

        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOMove(midPos, moveDuration / 2f).SetEase(Ease.OutQuad));
        seq.Append(transform.DOMove(movePos, moveDuration / 2f).SetEase(Ease.InQuad));
    }

    private void MoveBack()
    {
        transform.rotation = Quaternion.LookRotation(Vector3.back);
    }

    private void MoveForward(Vector3 movePos)
    {
        transform.rotation = Quaternion.LookRotation(Vector3.forward);
        animator.SetTrigger("Attack");
        Vector3 startPos = transform.position;
        Vector3 midPos = Vector3.Lerp(startPos, movePos, lerp);
        midPos.y += jumpHeight;
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOMove(midPos, moveDuration / 2f).SetEase(Ease.OutQuad));
        seq.Append(transform.DOMove(movePos, moveDuration / 2f).SetEase(Ease.InQuad));
    }
}