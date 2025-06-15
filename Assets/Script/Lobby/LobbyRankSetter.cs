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
        if (Main.Ins.MainData.RankData.LoadComplete)
        {
            Set();
        }
        else
        {
            Main.Ins.MainData.RankData.OnLoadComplete.Subscribe((_) =>
            {
                Set();
            }).AddTo(this);
        }
        Main.Ins.MainGame.OnBegin.Subscribe((_) =>
        {
            gameObject.SetActive(false);
        }).AddTo(this);
        Main.Ins.MainGame.OnEnd.Subscribe((_) =>
        {
            gameObject.SetActive(true);
        }).AddTo(this);
    }
    
    
    private async void Set()
    {
        var rankerList = Main.Ins.MainData.RankData.GetRankList(Tier.Purple);

        for (int i = 0; i < rankers.Length; i++)
        {
            rankers[i].nameText.text = rankerList[i].Name;

            if (rankers[i].player != null)
                Destroy(rankers[i].player);

            UserData.User user = await Main.Ins.MainData.UserData.LoadUserData(rankerList[i].UserId);
            int skin = user?.Skin ?? 0;

            rankers[i].player = Instantiate(Resources.Load<GameObject>(
                    $"{ResourcesPath.PlayerPath}_{skin}"),
                rankers[i].playerPos);

            rankers[i].player.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
    }

}
