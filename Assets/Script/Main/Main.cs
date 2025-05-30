using System;
using System.Collections;
using System.Collections.Generic;
using Firebase.Extensions;
using UniRx;
using UnityEngine;

public class Main : MonoBehaviour
{
    private static Main _ins;   
    public static Main Ins
    {
        get
        {
            if (_ins == null)
            {
                _ins = FindObjectOfType<Main>();
            }
            return _ins;
        }
    }

    public MainGame MainGame { get; private set; }
    public MainTime MainTime { get; private set; }
    public MainData MainData { get; private set; }
    public MainUser MainUser { get; private set; }
    
    public bool LoadComplete { get; private set; }
    public IObservable<Unit> OnLoadComplete => _onLoadComplete;
    private Subject<Unit> _onLoadComplete = new Subject<Unit>();

    public Transform loadCanvas;
    private void Awake()
    {
        MainGame = GetComponent<MainGame>();
        MainTime = GetComponent<MainTime>();
        MainData = GetComponent<MainData>();
        MainUser = GetComponent<MainUser>();
        
        Initialize();
    }

    private async void Initialize()
    {
        await Login.Ins.LoginUser();
        if (Login.Ins.IsNewUser)
        {
            var nickName = Instantiate(Resources.Load<NicknameSetter>($"{ResourcesPath.NickNamePath}"),loadCanvas);
            await nickName.OpenNickName();
        }
        await MainData.Initialize();
        await MainUser.Initialize();

        LoadComplete = true;
        _onLoadComplete.OnNext(Unit.Default);
        _onLoadComplete.OnCompleted();
        _onLoadComplete.Dispose();
        _onLoadComplete = null;
    }
}
