using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private DataTable<EnemyData> _enemyData = new();
    private Vector3 _pos;

    public void Initialize()
    {
        _enemyData.Load();
    }

    public void Follow(Transform target)
    {
        
    }
}
