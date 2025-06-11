using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UniRx;
using UnityEngine;

public enum EnemyState
{
    Climb,
    Catch,
}
public class Enemy : MonoBehaviour
{
    private DataTable<EnemyData> _enemyData = new();
    private Vector3Int _pos;
    
    private ColorMatchContent _content;
    private Player _player;
    
    private Tween _moveTween;

    private EnemyState _state;
    public void Initialize()
    {
        _enemyData.Load();
        _content = Main.Ins.MainGame.GetGame<ColorMatchContent>();
        _player = _content.Player;
        _pos = Vector3Int.zero;
        _state = EnemyState.Climb;
        Move();
    }
    
    private void Move()
    {
        if (!_content.IsEndGame)
        {
            _state = EnemyState.Climb;
            if (_pos.x == _content.AnswerList[_pos.y+1])
            {
                MoveY();
            }
            else
            {
                MoveX();
            }
        }
    }

    private void MoveX()
    {
        var targetX = _content.AnswerList[_pos.y+1];
        var targetPos = new Vector3Int(targetX, _pos.y, _pos.z);
        var blockCount = Mathf.Abs(targetPos.x - _pos.x);
        float totalDuration = blockCount / _enemyData.GetValue(_content.Level).speed;
        LookAtDirection(targetPos);
        _moveTween?.Kill();
        _moveTween = transform.DOMove(targetPos, totalDuration).SetEase(Ease.InQuad).OnComplete(() =>
            {
                _pos = targetPos;
                if (_player.GetPos().y == _pos.y)
                {
                    Catch(_player.GetPos());
                }
                else
                {
                    Move();
                }
            });
    }

    private void MoveY()
    {
        var targetPos = new Vector3Int(_pos.x, _pos.y+1, _pos.z+1);
        var speed = _enemyData.GetValue(_content.Level).speed;
        var duration = 1f / speed;
        Vector3 midPos = Vector3.Lerp(_pos, targetPos, 1.0f);
        midPos.y += 0.2f;
        
        LookAtDirection(targetPos);
        var moveTween = DOTween.Sequence();
        moveTween.Append(transform.DOMove(midPos, duration / 2f).SetEase(Ease.OutQuad));
        moveTween.Append(transform.DOMove(targetPos, duration / 2f).SetEase(Ease.InQuad));
        moveTween.OnComplete(() =>
        {
            _pos = targetPos;
            if (_player.GetPos().y == _pos.y)
            {
                Catch(_player.GetPos());
            }
            else
            {
                Move();
            }
        });
    }
    
    private void Catch(Vector3Int target)
    {
        if (_content.IsEndGame)
        {
            return;
        }
        _moveTween?.Kill();
        _state = EnemyState.Catch;

        float speed = _enemyData.GetValue(_content.Level).speed;
        var distance = Mathf.Abs(target.x - _pos.x);
        float duration = distance / speed;
        LookAtDirection(target);
        _moveTween = transform.DOMove((Vector3)target, duration)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() =>
            {
                _pos = target;

                if (_player.GetPos().y == _pos.y)
                {
                    Catch(_player.GetPos());
                }
                else
                {
                    Move();
                }
            });
    }
    private void LookAtDirection(Vector3 targetPos)
    {
        Vector3 direction = (targetPos - transform.position).normalized;
        direction.y = 0f;
        if (direction == Vector3.zero)
        {
            return;
        }
        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
        transform.DORotateQuaternion(targetRotation, 0.2f).SetEase(Ease.InOutSine);
    }
    private void OnTriggerEnter(Collider other)
    {
        _moveTween?.Kill();
        _content.Fail();
    }
}
