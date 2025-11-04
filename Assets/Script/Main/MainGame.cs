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

    private InGame _inGame;

    public InGame InGame
    {
        get => _inGame;
        private set => _inGame = value;
    }
    private void Awake()
    {           
        if (Main.Ins.LoadComplete)
        {
            Subscribe();
        }
        else
        {
            Main.Ins.OnLoadComplete.Subscribe((_) =>
            {
                Subscribe();
            }).AddTo(this);
        }
    }

    private void Subscribe()
    {
        LoadGame();
        if (UIMain.Ins.LoadComplete)
        {
        }
        else
        {
            UIMain.Ins.OnLoadComplete.Subscribe((_) =>
            {
            }).AddTo(this);
        }
    }
    private void LoadGame()
    {
        var prefab = Resources.Load<InGame>($"{ResourcesPath.GamePath}{"ColorMatch"}");
        InGame = Instantiate(prefab);
        InGame.Initialized();
        InGame.gameObject.SetActive(false);
    }
    
    public void EnterGame()
    {
        UIMain.Ins.UiLobby.gameObject.SetActive(false);
        UIMain.Ins.UIColorMatch.gameObject.SetActive(true);
        InGame.Begin();
        _onBegin.OnNext(Unit.Default);
        InGame.gameObject.SetActive(true);
    }

    public void RetryGame()
    {
        InGame.End();
        _onEnd.OnNext(Unit.Default);
        InGame.Begin();
        _onBegin.OnNext(Unit.Default);
        if ((Main.Ins.MainData.UserData.UserInfo.PlayCount+1) % 3 == 0)
        {
            AdsManager.Ins.TryShowAdOnGameOver();
        }
    }

    public void ReturnToLobby()
    {
        UIMain.Ins.UiLobby.gameObject.SetActive(true);
        UIMain.Ins.UIColorMatch.gameObject.SetActive(false);
        InGame.End();
        _onEnd.OnNext(Unit.Default);
        InGame.gameObject.SetActive(false);
        if ((Main.Ins.MainData.UserData.UserInfo.PlayCount+1) % 3 == 0)
        {
            AdsManager.Ins.TryShowAdOnGameOver();
        }
    }
}
