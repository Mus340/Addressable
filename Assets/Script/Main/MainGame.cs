using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class MainGame : MonoBehaviour
{
    public IObservable<Unit> OnBegin => _onBegin;
    private ISubject<Unit> _onBegin = new Subject<Unit>();
    
    public IObservable<Unit> OnEnd => _onEnd;
    private ISubject<Unit> _onEnd = new Subject<Unit>();

    private GameContent _gameContent;
    
    private void Awake()
    {           
        if (Main.Ins.LoadComplete)
        {
            LoadGame();
        }
        else
        {
            Main.Ins.OnLoadComplete.Subscribe((_) =>
            {
                LoadGame();
            }).AddTo(this);
        }
    }

    private void LoadGame()
    {
        var prefab = Resources.Load<GameContent>($"{ResourcesPath.GamePath}{"ColorMatch"}");
        _gameContent = Instantiate(prefab);
        _gameContent.Initialized();
        _gameContent.gameObject.SetActive(false);
    }
    
    public void EnterGame()
    {
        UIMain.Ins.UiLobby.gameObject.SetActive(false);
        UIMain.Ins.UIColorMatch.gameObject.SetActive(true);
        _gameContent.Begin();
        _onBegin.OnNext(Unit.Default);
        _gameContent.gameObject.SetActive(true);
    }

    public void RetryGame()
    {
        _gameContent.End();
        _onEnd.OnNext(Unit.Default);
        _gameContent.Begin();
        _onBegin.OnNext(Unit.Default);
        //AdsManager.Ins.TryShowAdOnGameOver();
    }

    public void ReturnToLobby()
    {
        UIMain.Ins.UiLobby.gameObject.SetActive(true);
        UIMain.Ins.UIColorMatch.gameObject.SetActive(false);
        _gameContent.End();
        _onEnd.OnNext(Unit.Default);
        _gameContent.gameObject.SetActive(false);
        //AdsManager.Ins.TryShowAdOnGameOver();
    }

    public T GetGame<T>() where T : GameContent
    {
        return _gameContent as T;
    }
}
