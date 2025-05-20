using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameType
{
    ColorMatch,
}
public class GameContentProvider : MonoBehaviour
{
    private Dictionary<GameType, GameContent> _games;
    
    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        _games = new Dictionary<GameType, GameContent>();
        foreach (GameType gameType in Enum.GetValues(typeof(GameType)))
        {
            var prefab = Resources.Load<GameContent>($"{ResourcesPath.GamePath}{gameType}");
            var game = Instantiate(prefab, this.transform);
            game.gameObject.SetActive(false);
            _games.Add(gameType, game);
        }
    }

    public GameContent GetGameContent(GameType type)
    {
        return _games.GetValueOrDefault(type);
    }
    
    public T GetGameContent<T>(GameType type) where T : GameContent
    {
        if (_games.TryGetValue(type, out var content))
        {
            return content as T;
        }
        return null;
    }
}
