using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;

public class LobbyRankSetter : MonoBehaviour
{
    [Serializable]
    public class LobbyRanker
    {
        public Transform playerPos;
        public TextMeshPro nameText;
        public GameObject player;
    }
    public LobbyRanker[] rankers;
    private void Awake()
    {
        if (Main.Ins.LoadComplete)
        {
            Subscribe();
            Set();
        }
        else
        {
            Main.Ins.OnLoadComplete.Subscribe((_) =>
            {
                Subscribe();
                Set();
            }).AddTo(this);
        }
    }

    private void Subscribe()
    {
        Main.Ins.MainGame.OnBegin.Subscribe((_) =>
        {
            gameObject.SetActive(false);
        }).AddTo(this);
        Main.Ins.MainGame.OnEnd.Subscribe((_) =>
        {
            gameObject.SetActive(true);
        }).AddTo(this);
    }
    private void Set()
    {
        var rankerList = Main.Ins.MainData.RankData.GetRankList(Tier.Purple);
        for (int i = 0; i < rankers.Length; i++)
        {
            rankers[i].nameText.text = rankerList[i].Name;
            if (rankers[i].player != null)
            {
                Destroy(rankers[i].player);
            }
            rankers[i].player =
                Instantiate(Resources.Load<GameObject>(
                    $"{ResourcesPath.PlayerPath}_{rankerList[i].Skin}"),
                    rankers[i].playerPos);
            rankers[i].player.transform.rotation = new Quaternion(0f, 180f, 0f, 0);
        }
    }
}
