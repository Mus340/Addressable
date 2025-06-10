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

    private void MoveX()
    {
        var targetX = _content.AnswerList[_pos.y+1];
        var targetPos = new Vector3Int(targetX, _pos.y, _pos.z);
        var blockCount = Mathf.Abs(targetPos.x - _pos.x);
        var totalDuration = _enemyData.GetValue(_content.Level).speed * blockCount;

        _moveTween?.Kill();
        _moveTween = transform.DOMove(targetPos, totalDuration).OnComplete(() =>
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
        Vector3 midPos = Vector3.Lerp(_pos, targetPos, 1.0f);
        midPos.y += 0.2f;
        
        var moveTween = DOTween.Sequence();
        moveTween.Append(transform.DOMove(midPos, speed / 2f).SetEase(Ease.OutQuad));
        moveTween.Append(transform.DOMove(targetPos, speed / 2f).SetEase(Ease.InQuad));
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
        _moveTween?.Kill();
        _state = EnemyState.Catch;
        var speed = _enemyData.GetValue(_content.Level).speed;
        _moveTween = transform.DOMove(target, speed)
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
    
    private void OnTriggerEnter(Collider other)
    {
        _content.Fail();
    }
}
