using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class UILoading : MonoBehaviour
{
    private void Awake()
    {
        Main.Ins.OnLoadComplete.Subscribe((_) =>
        {
            Destroy(this.gameObject);
        }).AddTo(this);
    }
}
