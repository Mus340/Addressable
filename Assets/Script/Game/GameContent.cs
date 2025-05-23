using System;
using UniRx;
using UnityEngine;

public abstract class GameContent : MonoBehaviour
{
    public abstract void Initialized();
    public abstract void End();
    public abstract void Begin();
}