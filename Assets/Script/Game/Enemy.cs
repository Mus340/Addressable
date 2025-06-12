using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;

public enum EnemyState
{
    Climb,
    Catch,
}
public class Enemy : MonoBehaviour
{
    public IObservable<Unit> OnNext => _onNext;
    private ISubject<Unit> _onNext = new Subject<Unit>();
    
    public IObservable<Unit> OnCatch => _onCatch;
    private ISubject<Unit> _onCatch = new Subject<Unit>();
    public Vector3Int Pos { get; private set; }

    private DataTable<EnemyData> _enemyData = new();
    
    private ColorMatchContent _content;
    private Player _player;
    private Tween _moveTween;
    private EnemyState _state;
    
    public void Initialize()
    {
        _enemyData.Load();
        _content = Main.Ins.MainGame.GetGame<ColorMatchContent>();
        _player = _content.Player;
        Pos = Vector3Int.zero;
        _state = EnemyState.Climb;
        StartCoroutine(SpawnEffect());
    }

    private IEnumerator SpawnEffect()
    {
        Main.Ins.MainEffect.Play(EffectType.EnemySpawn, new Vector3(Pos.x,Pos.y+1.2f,Pos.z));
        yield return new WaitForSeconds(0.5f);
        Move();
    }
    private void Move()
    {
        if (!_content.IsEndGame)
        {
            _state = EnemyState.Climb;
            if (Pos.x == _content.AnswerList[Pos.y+1])
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
        var targetX = _content.AnswerList[Pos.y+1];
        var targetPos = new Vector3Int(targetX, Pos.y, Pos.z);
        var blockCount = Mathf.Abs(targetPos.x - Pos.x);
        float totalDuration = blockCount / _enemyData.GetValue(_content.Level).speed;
        LookAtDirection(targetPos);
        _moveTween?.Kill();
        _moveTween = transform.DOMove(targetPos, totalDuration).SetEase(Ease.InQuad).OnComplete(() =>
            {
                Pos = targetPos;
                if (_player.GetPos().y == Pos.y)
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
        var targetPos = new Vector3Int(Pos.x, Pos.y+1, Pos.z+1);
        var speed = _enemyData.GetValue(_content.Level).speed;
        var duration = 1f / speed;
        Vector3 midPos = Vector3.Lerp(Pos, targetPos, 1.0f);
        midPos.y += 0.2f;
        
        LookAtDirection(targetPos);
        var moveTween = DOTween.Sequence();
        moveTween.Append(transform.DOMove(midPos, duration / 2f).SetEase(Ease.OutQuad));
        moveTween.Append(transform.DOMove(targetPos, duration / 2f).SetEase(Ease.InQuad));
        moveTween.OnComplete(() =>
        {
            Pos = targetPos;
            _onNext.OnNext(Unit.Default);
            if (_player.GetPos().y == Pos.y)
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
        var distance = Mathf.Abs(target.x - Pos.x);
        float duration = distance / speed;
        LookAtDirection(target);
        _moveTween = transform.DOMove((Vector3)target, duration)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() =>
            {
                Pos = target;

                if (_player.GetPos().y == Pos.y)
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
        _onCatch.OnNext(Unit.Default);
        PlayCatchEffect();
    }

    private void PlayCatchEffect()
    {
        _player.transform.SetParent(this.transform);

        Vector3 startPos = transform.position;
        Vector3 toPlayer = _player.transform.position - transform.position;
        toPlayer.y = 0f;
        toPlayer.Normalize();
        Vector3[] directions = { Vector3.forward, Vector3.back, Vector3.left, Vector3.right };
        Vector3 closest = directions[0];
        float maxDot = Vector3.Dot(toPlayer, closest);

        for (int i = 1; i < directions.Length; i++)
        {
            float dot = Vector3.Dot(toPlayer, directions[i]);
            if (dot > maxDot)
            {
                maxDot = dot;
                closest = directions[i];
            }
        }
        Vector3 flyDir = (closest == Vector3.left || closest == Vector3.right) ? closest : Vector3.left;
        PlayParabolaAndFly(startPos, flyDir);
    }

    private void PlayParabolaAndFly(Vector3 startPos, Vector3 flyDir)
    {
        Vector3 targetPos = new Vector3(startPos.x, startPos.y + 5f, startPos.z);
        Vector3 midPos = Vector3.Lerp(startPos, targetPos, 0.5f);
        Vector3 flyPos = midPos + flyDir * 10f;

        Quaternion targetRotation = Quaternion.LookRotation(flyDir, Vector3.up);

        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOMove(midPos, 0.7f).SetEase(Ease.OutQuad));
        seq.Append(transform.DORotateQuaternion(targetRotation, 0.1f).SetEase(Ease.InOutSine));
        seq.Append(transform.DOMove(flyPos, 0.7f).SetEase(Ease.InQuad));
    }
}
