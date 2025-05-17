using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameContent<T> : MonoBehaviour
{
    public abstract void OnInitialized();
    public abstract void OnEnd();
    public abstract void OnBegin();
    public abstract void OnSubscribe();
}
