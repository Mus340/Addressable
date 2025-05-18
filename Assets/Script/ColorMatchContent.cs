using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;


public class ColorMatchContent : MonoBehaviour,IGameContent
{
    public CompositeDisposable _disposable = new CompositeDisposable();

    public IObservable<Void> OnStart => _onStart;
    public ISubject<Void> _onStart = new Subject<Void>();
    
    public void OnInitialized()
    {
        _onStart.OnNext(Void.Empty);
    }

    public void OnEnd()
    {
        
    }

    public void OnBegin()
    {
        
    }

    public void OnSubscribe()
    {
    }
}
