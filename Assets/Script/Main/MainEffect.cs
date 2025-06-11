using System;
using System.Collections;
using System.Collections.Generic;
using CartoonFX;
using UnityEngine;

public enum EffectType
{
    Hit,
    EnemySpawn,
}
public class MainEffect : MonoBehaviour
{
    public Transform parent;
    private Dictionary<EffectType, CFXR_Effect> _effects;

    public void Initialize()
    {
        _effects = new Dictionary<EffectType, CFXR_Effect>();
        foreach (EffectType type in Enum.GetValues(typeof(EffectType)))
        {
            var prefab = Resources.Load<CFXR_Effect>($"{ResourcesPath.EffectPath}{type}");
            var effect = Instantiate(prefab, parent);
            effect.gameObject.SetActive(false);
            _effects.Add(type, effect);
        }
    }

    public void Play(EffectType type, Vector3 pos)
    {
        _effects[type].transform.position = pos;
        _effects[type].gameObject.SetActive(true);
    }

    public void Stop(EffectType type)
    {
        _effects[type].gameObject.SetActive(false);
    }
}
