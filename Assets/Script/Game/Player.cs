using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public enum PlayerMove
{
    Left,
    Right,
    Up,
}
public class Player : MonoBehaviour
{
    public void Move(Vector3 pos)
    {
        transform.position = pos;
    }
}
