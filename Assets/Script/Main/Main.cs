using System;
using System.Collections;
using System.Collections.Generic;
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
    
    public bool LoadComplete { get; private set; } = false;
    private AsyncSubject<Unit> _onLoadComplete = new AsyncSubject<Unit>();
    public IObservable<Unit> OnLoadComplete => _onLoadComplete;
    
    private void Awake()
    {
        Initialize();
        OnLoadComplete.Subscribe((_) =>
        {
            LoadComplete = true;
        }).AddTo(this);
        _onLoadComplete.OnNext(Unit.Default);
        _onLoadComplete.OnCompleted();
        _onLoadComplete.Dispose();
        _onLoadComplete = null;
    }


    private void Initialize()
    {
        MainGame = FindObjectOfType<MainGame>();
        MainTime = FindObjectOfType<MainTime>();
    }
}
