using System;
using FlaxEngine;
using FlaxEngine.Networking;

/// <summary>
/// Game service with packets and network connections handling
/// </summary>
public class NetworkSession : GamePlugin
{
    private PacketRegistry _packetRegistry;
    private ConnectionRegistry _connRegistry;
    private NetworkPeer _peer;
    private bool _isServer;
    private bool _isConnected;

    public bool IsServer => _isServer;

    public bool IsConnected => _isConnected;

    public NetworkSession()
    {
        _description = new PluginDescription
        {
            Name = "NetworkPlugin",
            Description = "Network logic"
        };
    }

    public override void Initialize()
    {
        base.Initialize();
        _isConnected = false;
        _packetRegistry = new PacketRegistry();

        _packetRegistry.Register<ConnectionRequestPacket>();
        _packetRegistry.Register<ConnectionResponsePacket>();
        _packetRegistry.Register<ChatMessagePacket>();
        _packetRegistry.Register<PlayerConnectedPacket>();
        _packetRegistry.Register<PlayerDisconnectedPacket>();
        _packetRegistry.Register<PlayerListPacket>();
        _packetRegistry.Register<PlayerTransformPacket>();
        _packetRegistry.Register<PlayersTransformPacket>();

        _connRegistry = new ConnectionRegistry();
        Scripting.Update += Update;
    }

    public override void Deinitialize()
    {
        base.Deinitialize();
        Scripting.Update -= Update;
        Disconnect();
        _isConnected = false;
        _isServer = false;
        if (_instance == this)
            _instance = null;
    }

    public void Update()
    {
        if (!_isConnected)
            return;

        if (_isServer)
        {
            while (_peer.PopEvent(out var eventData))
            {
                if (eventData.EventType == NetworkEventType.Message)
                {
                    //Debug.Log($"Sender={eventData.Sender.ConnectionId} is sending a packet !");
                    _packetRegistry.Receive(ref eventData, _isServer);
                    _peer.RecycleMessage(eventData.Message);
                }
                else if (eventData.EventType == NetworkEventType.Connected)
                {
                    Debug.Log($"Sender={eventData.Sender.ConnectionId} is connected !");
                    _connRegistry.Add(ref eventData.Sender, GameSession.Instance.AddPlayer());
                }
                else if (eventData.EventType == NetworkEventType.Disconnected ||
                         eventData.EventType == NetworkEventType.Timeout)
                {
                    Debug.Log($"Sender={eventData.Sender.ConnectionId} is disconnected !");
                    var g = GuidByConn(ref eventData.Sender);
                    GameSession.Instance.RemovePlayer(ref g);
                    _connRegistry.Remove(ref eventData.Sender);
                    _isConnected = false;
                    _isServer = false;
                }
            }
        }
        else
        {
            while (_peer.PopEvent(out var eventData))
            {
                if (eventData.EventType == NetworkEventType.Message)
                {
                    _packetRegistry.Receive(ref eventData, _isServer);
                    _peer.RecycleMessage(eventData.Message);
                }
                else if (eventData.EventType == NetworkEventType.Disconnected ||
                         eventData.EventType == NetworkEventType.Timeout)
                {
                    Debug.Log($"Sender={eventData.Sender.ConnectionId} is disconnected !");
                    NetworkPeer.ShutdownPeer(_peer);
                    var g = GuidByConn(ref eventData.Sender);
                    GameSession.Instance.RemovePlayer(ref g);
                    _isConnected = false;
                    _isServer = false;
                }
                else if (eventData.EventType == NetworkEventType.Connected)
                {
                    Send(new ConnectionRequestPacket() { Username = GameSession.Instance.LocalPlayer.Name }, NetworkChannelType.ReliableOrdered);
                }
            }
        }
    }

    public bool Host(string username, ushort port)
    {
        if (_isConnected)
            Disconnect();

        _connRegistry.Clear();
        GameSession.Instance.LocalPlayer.Name = username;
        GameSession.Instance.LocalPlayer.ID = Guid.NewGuid();
        _peer = NetworkPeer.CreatePeer(new NetworkConfig
        {
            NetworkDriver = new ENetDriver(),
            ConnectionsLimit = 32,
            MessagePoolSize = 256,
            MessageSize = 1400,
            Address = "any",
            Port = port
        });
        if (!_peer.Listen())
            return true;
        _isConnected = true;
        _isServer = true;
        return false;
    }

    public bool Connect(string username, string address, ushort port)
    {
        if (_isConnected)
            Disconnect();

        _connRegistry.Clear();
        _isServer = false;
        _peer = NetworkPeer.CreatePeer(new NetworkConfig
        {
            NetworkDriver = new ENetDriver(),
            ConnectionsLimit = 32,
            MessagePoolSize = 256,
            MessageSize = 1400,
            Address = address,
            Port = port
        });
        GameSession.Instance.LocalPlayer.Name = username;
        if (!_peer.Connect())
            return true;
        _isConnected = true;
        _isServer = false;
        return false;
    }

    public void Disconnect()
    {
        if (_isConnected)
        {
            if (!_isServer)
                _peer.Disconnect();
            NetworkPeer.ShutdownPeer(_peer);
        }

        _isConnected = false;
        _isServer = false;
    }

    public void Send(NetworkPacket packet, NetworkChannelType type)
    {
        if (!_isConnected)
            return;
        var msg = _peer.BeginSendMessage();
        _packetRegistry.Send(packet, ref msg);
        if (_isServer)
            _peer.EndSendMessage(type, msg, _connRegistry.ToArray());
        else
            _peer.EndSendMessage(type, msg);
    }

    public void Send(NetworkPacket packet, NetworkChannelType type, ref NetworkConnection conn)
    {
        if (!_isServer || !_isConnected)
            return;
        var msg = _peer.BeginSendMessage();
        _packetRegistry.Send(packet, ref msg);
        _peer.EndSendMessage(type, msg, conn);
    }

    public void SendAll(NetworkPacket packet, NetworkChannelType type)
    {
        if (!_isServer || !_isConnected)
            return;
        var msg = _peer.BeginSendMessage();
        _packetRegistry.Send(packet, ref msg);
        _peer.EndSendMessage(type, msg, _connRegistry.ToArray());
    }

    public Guid GuidByConn(ref NetworkConnection conn)
    {
        return _connRegistry.GuidByConn(ref conn);
    }

    public void DisconnectPlayer(Player player)
    {
        DisconnectPlayer(ref player.ID);
    }

    public void DisconnectPlayer(ref Guid guid)
    {
        if (!_isServer || !_isConnected)
            return;
        PlayerDisconnectedPacket pdp = new PlayerDisconnectedPacket();
        pdp.ID = guid;
        SendAll(pdp, NetworkChannelType.ReliableOrdered);
        _peer.Disconnect(_connRegistry.ConnByGuid(ref guid));
        _connRegistry.Remove(ref guid);
        GameSession.Instance.RemovePlayer(ref guid);
    }

    public void RemovePlayer(Player player)
    {
        RemovePlayer(ref player.ID);
    }

    public void RemovePlayer(ref Guid guid)
    {
        if (!_isServer || !_isConnected)
            return;
        _peer.Disconnect(_connRegistry.ConnByGuid(ref guid));
        _connRegistry.Remove(ref guid);
    }

    private static NetworkSession _instance;

    public static NetworkSession Instance
    {
        get
        {
            if (_instance == null)
                _instance = PluginManager.GetPlugin<NetworkSession>();
            return _instance;
        }
    }
}
