using System;
using UniRx;
using UnityEngine;

public abstract class UIContentPanel : MonoBehaviour
{
    protected CompositeDisposable Disposable {get; private set;}
    
    protected abstract void Initialize();
    protected abstract void Enter();
    
    private void Awake()
    {
        if (Main.Ins.LoadComplete)
        {
            OnInitialize();
        }
        else
        {
            Main.Ins.OnLoadComplete.Subscribe((_) =>
            {
                OnInitialize();
            }).AddTo(this);
        }
    }

    private void OnInitialize()
    {
        Initialize();
        Main.Ins.MainGame.OnBegin.Subscribe((type) =>
        {
            Disposable = new CompositeDisposable();
            Enter();
        }).AddTo(this);
        
        Main.Ins.MainGame.OnEnd.Subscribe((_) =>
        {
            Disposable?.Dispose();
        }).AddTo(this);
    }
}
