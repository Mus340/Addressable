using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class GameLoader : MonoBehaviour
{
    private void Awake()
    {
        if (Main.Ins.LoadComplete)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Main.Ins.OnLoadComplete.Subscribe((_) =>
            {
                Destroy(this.gameObject);
            }).AddTo(this);
        }
    }
}
