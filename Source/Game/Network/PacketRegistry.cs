using System;
using System.Collections.Generic;
using FlaxEngine;
using FlaxEngine.Networking;

public class PacketRegistry
{
    private Dictionary<int, Type> _packets = new Dictionary<int, Type>();

    public void Register<T>() where T : NetworkPacket
    {
        // if it's already registered
        _packets.Remove(typeof(T).Name.GetDeterministicHashCode());
        _packets.Add(typeof(T).Name.GetDeterministicHashCode(), typeof(T));
    }

    public void Receive(ref NetworkEvent eventData, bool isServer = false)
    {
        int t = eventData.Message.ReadInt32();
        if (!_packets.ContainsKey(t))
        {
            Debug.LogWarning($"Packet not registered, Type=" + t);
            return;
        }

        var type = _packets[t];
        var p = (NetworkPacket)Activator.CreateInstance(type);
        p.Sender = eventData.Sender;
        //Debug.Log($"Message Sender={eventData.Sender.ConnectionId} Type={type.Name}");
        p.Deserialize(ref eventData.Message);
        if (isServer)
            p.ServerHandler(ref eventData.Sender);
        else
            p.ClientHandler();
    }

    public void Send(NetworkPacket packet, ref NetworkMessage msg)
    {
        msg.WriteInt32(packet.GetType().Name.GetDeterministicHashCode());
        packet.Serialize(ref msg);
    }
}
