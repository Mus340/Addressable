using System;
using System.Collections;
using System.Collections.Generic;
using CartoonFX;
using DG.Tweening;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Sequence = DG.Tweening.Sequence;


public class Player : MonoBehaviour
{
    public PlayerController playerController;
    public Animator animator;
    public float jumpHeight;
    public float moveDuration;
    public float lerp;

    private ColorMatchContent _content;
    private Vector3Int _pos;

    public Vector3Int GetPos() => _pos;
    private Sequence _sequence;
    
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
                Move(PlayerMove.Back, new Vector3(_pos.x, _pos.y, _pos.z));
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
        }).AddTo(this);

        _content.SpawnEnemy.Subscribe((_) =>
        {
            _content.Enemy.OnCatch.Subscribe((_) =>
            {
                _sequence?.Kill();
            }).AddTo(this);
        }).AddTo(this);
    }
    
    private void Check(PlayerMove move)
    {
        if (!_content.IsEndGame)
        {
            if (move == PlayerMove.Left && (_pos.x-1) >= 0)
            {
                _pos.x--;
                Move(move, new Vector3(_pos.x, _pos.y, _pos.z));
                if (_content.Level > 0)
                {
                    _content.AddScore(1);
                }
            }
            else if (move == PlayerMove.Right && (_pos.x + 1) < _content.LevelData.GetValue(_pos.y).cube_count)
            {
                _pos.x++;
                Move(move, new Vector3(_pos.x, _pos.y, _pos.z));
                if (_content.Level > 0)
                {
                    _content.AddScore(1);
                }
            }
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

        _sequence = DOTween.Sequence();
        _sequence.Append(transform.DOMove(midPos, moveDuration / 2f).SetEase(Ease.OutQuad));
        _sequence.Append(transform.DOMove(movePos, moveDuration / 2f).SetEase(Ease.InQuad));
    }

    private void MoveRight(Vector3 movePos)
    {
        transform.rotation = Quaternion.LookRotation(Vector3.right);
        animator.SetTrigger("Attack");

        Vector3 startPos = transform.position;
        Vector3 midPos = Vector3.Lerp(startPos, movePos, lerp);
        midPos.y += jumpHeight;

        _sequence = DOTween.Sequence();
        _sequence.Append(transform.DOMove(midPos, moveDuration / 2f).SetEase(Ease.OutQuad));
        _sequence.Append(transform.DOMove(movePos, moveDuration / 2f).SetEase(Ease.InQuad));
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
        _sequence = DOTween.Sequence();
        _sequence.Append(transform.DOMove(midPos, moveDuration / 2f).SetEase(Ease.OutQuad));
        _sequence.Append(transform.DOMove(movePos, moveDuration / 2f).SetEase(Ease.InQuad));
    }

    public void PlayCrashEffect()
    {        
        Main.Ins.MainEffect.Play(EffectType.Hit, new Vector3(_pos.x, _pos.y+0.7f, _pos.z));
        var movePos = new Vector3(_pos.x, _pos.y+(transform.localScale.y/2.0f), _pos.z);
        transform.rotation = Quaternion.LookRotation(Vector3.forward);
        animator.SetTrigger("Attack");

        Vector3 startPos = transform.position;
        Vector3 midPos = Vector3.Lerp(startPos, movePos, lerp);
        midPos.y += 0.5f;
        
        _sequence = DOTween.Sequence();

        _sequence.Append(transform.DOMove(midPos, moveDuration / 2f).SetEase(Ease.OutQuad));
        _sequence.Append(transform.DOMove(movePos, moveDuration / 2f).SetEase(Ease.InQuad));

        startPos.y += 0.3f;
        startPos.z += 0.2f;
        
        _sequence.Append(transform.DOMove(startPos, 0.1f).SetEase(Ease.OutQuad));

        Quaternion fallRotation = Quaternion.Euler(-90f, transform.rotation.eulerAngles.y, 0f);
        _sequence.Join(transform.DORotateQuaternion(fallRotation, 0.2f).SetEase(Ease.InQuad));
    }
}