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
    public MainCamera MainCamera { get; private set; } 
    public bool LoadComplete { get; private set; }
    public IObservable<Unit> OnLoadComplete => _onLoadComplete;
    private Subject<Unit> _onLoadComplete = new Subject<Unit>();

    public Transform nickNameParent;
    private void Awake()
    {
        MainGame = GetComponent<MainGame>();
        MainTime = GetComponent<MainTime>();
        MainData = GetComponent<MainData>();
        MainCamera = GetComponent<MainCamera>();
        
        Initialize();
    }

    private async void Initialize()
    {
        await Login.Ins.LoginUser();
        await MainData.Initialize();
        if (Login.Ins.IsNewUser)
        {
            MainData.NameData.Initialize(MainData.Reference);
            var nickName = Instantiate(Resources.Load<NicknameSetter>($"{ResourcesPath.NickNamePath}"),nickNameParent);
            await nickName.OpenNickName();
        }
        await AdsManager.Ins.InitializeAdmobAsync();

        LoadComplete = true;
        _onLoadComplete.OnNext(Unit.Default);
        _onLoadComplete.OnCompleted();
        _onLoadComplete.Dispose();
        _onLoadComplete = null;
    }
}
