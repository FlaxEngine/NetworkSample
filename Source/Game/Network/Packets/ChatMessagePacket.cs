using System;
using FlaxEngine;
using FlaxEngine.Networking;
using Game;

public class ChatMessagePacket : NetworkPacket
{
    public string Message = string.Empty;

    public Guid Sender = Guid.Empty;

    public override void Serialize(ref NetworkMessage msg)
    {
        msg.WriteString(Message);
        msg.WriteBoolean(Sender != Guid.Empty);
        if (Sender != Guid.Empty)
            msg.WriteGuid(Sender);
    }
    
    public override void Deserialize(ref NetworkMessage msg)
    {
       Message = msg.ReadString();
       if (msg.ReadBoolean())
           Sender = msg.ReadGuid();
    }

    public override void ServerHandler(ref NetworkConnection sender)
    {
        Sender = PluginManager.GetPlugin<NetworkSession>().GuidByConn(ref sender);
        PluginManager.GetPlugin<GameSession>().AddChatMessage(Sender, Message);
        PluginManager.GetPlugin<NetworkSession>().SendAll(this, NetworkChannelType.ReliableOrdered);
    }

    public override void ClientHandler()
    {
        if (GameSession.Instance.LocalPlayer.ID == Sender)
            return;
        PluginManager.GetPlugin<GameSession>().AddChatMessage(Sender, Message);
    }
}