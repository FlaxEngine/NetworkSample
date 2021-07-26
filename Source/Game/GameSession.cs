using System;
using System.Collections.Generic;
using FlaxEngine;

public struct ChatMessage
{
    public Guid Sender;
    public string Message;
}

/// <summary>
/// Game service with players list including local player.
/// </summary>
public class GameSession : GamePlugin
{
    public delegate void OnPlayerAddedHandler(Player player);

    public event OnPlayerAddedHandler OnPlayerAdded;

    public delegate void OnPlayerRemovedHandler(Player player);

    public event OnPlayerRemovedHandler OnPlayerRemoved;

    public List<Player> Players = new List<Player>();
    public List<ChatMessage> ChatMessages = new List<ChatMessage>();

    public Player LocalPlayer;

    public override void Initialize()
    {
        base.Initialize();
        Players.Clear();
        LocalPlayer = new Player();
    }

    public override void Deinitialize()
    {
        if (_instance == this)
            _instance = null;
        base.Deinitialize();
    }

    public Player AddPlayer()
    {
        var p = new Player() {ID = Guid.NewGuid()};
        AddPlayer(p);
        return p;
    }

    public Player AddPlayer(ref Guid guid, string name)
    {
        var p = new Player() {ID = guid, Name = name};
        AddPlayer(p);
        return p;
    }

    public void AddPlayer(Player player)
    {
        Players.Add(player);
        OnPlayerAdded?.Invoke(player);
    }

    public bool RemovePlayer(ref Guid id)
    {
        for (var i = Players.Count - 1; i >= 0; i--)
        {
            if (Players[i].ID == id)
            {
                var p = Players[i];
                Players.RemoveAt(i);
                OnPlayerRemoved?.Invoke(p);
                return true;
            }
        }

        return false;
    }

    public void AddChatMessage(Guid sender, string message)
    {
        ChatMessages.Add(new ChatMessage() {Sender = sender, Message = message});
    }

    public Player GetPlayer(Guid guid)
    {
        if (LocalPlayer.ID == guid)
            return LocalPlayer;
        for (var i = 0; i < Players.Count; i++)
        {
            if (Players[i].ID == guid)
                return Players[i];
        }

        return null;
    }

    private static GameSession _instance;

    public static GameSession Instance
    {
        get
        {
            if (_instance == null)
                _instance = PluginManager.GetPlugin<GameSession>();
            return _instance;
        }
    }
}
