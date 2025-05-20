using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class Main : MonoBehaviour
{
    private static Main _ins;   
    public static Main Ins
    {
        get
        {
            if (_ins == null)
            {
                _ins = FindObjectOfType<Main>();
            }
            return _ins;
        }
    }

    public GameContentProvider GameContentProvider { get; private set; }
    public GameType? CurGameType { get; private set; }
    
    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        GameContentProvider = FindObjectOfType<GameContentProvider>();
    }

    public void EnterGame(GameType type)
    {
        if (CurGameType.HasValue)
        {
            return;
        }
        CurGameType = type;
        var game = GameContentProvider.GetGameContent(type);
        game.Initialized();
        game.Begin();
        game.gameObject.SetActive(true);
    }

    public void ReturnToLobby()
    {
        if (CurGameType.HasValue)
        {
            var game = GameContentProvider.GetGameContent(CurGameType.Value);
            game.End();
            game.gameObject.SetActive(false);
            CurGameType = null;
        }
    }
}
