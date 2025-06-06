using System;
using UniRx;
using UnityEngine;

public abstract class GameContent : MonoBehaviour
{
    public abstract void Initialized();
    public abstract void Begin();
    public abstract void End();
}