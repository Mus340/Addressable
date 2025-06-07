using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class MainCamera : MonoBehaviour
{
    public CinemachineVirtualCamera cineVirtual;
    public Vector3 followOffset;
    public float followDamping;

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
        var composer = cineVirtual.GetCinemachineComponent<CinemachineComposer>();
        if (composer != null)
        {
            composer.m_TrackedObjectOffset = Vector3.zero;
            composer.m_HorizontalDamping = 0f;
            composer.m_VerticalDamping = 0f;
        }
    }

}
