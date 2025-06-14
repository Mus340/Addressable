using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class MainCamera : MonoBehaviour
{
    public Camera lobbyCam;
    public CinemachineVirtualCamera cineVirtual;
    public Vector3 followOffset;
    public float followDamping;


    private void Awake()
    {
        if (Main.Ins.LoadComplete)
        {
            Subscribe();
        }
        else
        {
            Main.Ins.OnLoadComplete.Subscribe((_) =>
            {
                Subscribe();
            }).AddTo(this);
        }
    }

    private void Subscribe()
    {
        Main.Ins.MainGame.OnBegin.Subscribe((_) =>
        {
            lobbyCam.gameObject.SetActive(false);
            cineVirtual.gameObject.SetActive(true);
        }).AddTo(this);
        Main.Ins.MainGame.OnEnd.Subscribe((_) =>
        {
            lobbyCam.gameObject.SetActive(true);
            cineVirtual.gameObject.SetActive(false);
        }).AddTo(this);
    }
    
    public void Follow(Transform target)
    {
        if (cineVirtual == null)
        {
            return;
        }
        
        cineVirtual.Follow = target;
        cineVirtual.LookAt = target;

        var transposer = cineVirtual.GetCinemachineComponent<CinemachineTransposer>();
        if (transposer != null)
        {
            transposer.m_FollowOffset = followOffset;
            transposer.m_BindingMode = CinemachineTransposer.BindingMode.WorldSpace;
            transposer.m_XDamping = followDamping;
            transposer.m_YDamping = followDamping;
            transposer.m_ZDamping = followDamping;
        }
    }

    public void UnFollow()
    {
        cineVirtual.Follow = null;
        cineVirtual.LookAt = null;
    }

}
