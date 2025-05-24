using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class MainGame : MonoBehaviour
{
    public GameContentProvider GameContentProvider { get; private set; }

    public IObservable<GameType> OnBegin => _onBegin;
    private ISubject<GameType> _onBegin = new Subject<GameType>();
    
    public IObservable<Unit> OnEnd => _onEnd;
    private ISubject<Unit> _onEnd = new Subject<Unit>();
    
    public GameType? CurGameType { get; private set; }

    public void Initialize()
    {
        GameContentProvider = FindObjectOfType<GameContentProvider>();
        GameContentProvider.Initialize();
    }
    
    public void EnterGame(GameType type)
    {
        if (CurGameType.HasValue)
        {
            return;
        }
        CurGameType = type;
        var game = GameContentProvider.GetGameContent(type);
        game.Begin();
        _onBegin.OnNext(CurGameType.Value);
        game.gameObject.SetActive(true);
    }

    public void RetryGame()
    {
        if (CurGameType.HasValue)
        {
            var game = GameContentProvider.GetGameContent(CurGameType.Value);
            game.End();
            _onEnd.OnNext(Unit.Default);
            game.Begin();
            _onBegin.OnNext(CurGameType.Value);
        }
    }
    
    public void ReturnToLobby()
    {
        if (CurGameType.HasValue)
        {
            var game = GameContentProvider.GetGameContent(CurGameType.Value);
            game.End();
            _onEnd.OnNext(Unit.Default);
            game.gameObject.SetActive(false);
            CurGameType = null;
        }
    }
}
