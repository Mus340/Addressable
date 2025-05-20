using System;
using UniRx;
using UnityEngine;

public abstract class GameContent : MonoBehaviour
{
    public IObservable<Void> OnBegin => _onBegin;
    protected ISubject<Void> _onBegin = new Subject<Void>();
    
    public IObservable<Void> OnEnd => _onEnd;
    protected ISubject<Void> _onEnd = new Subject<Void>();

    public abstract void Initialized();
    public abstract void End();
    public abstract void Begin();
    public abstract void Subscribe();
}