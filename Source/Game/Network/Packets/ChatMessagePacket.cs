using System;
using FlaxEngine.Networking;

public class ChatMessagePacket : NetworkPacket
{
    public string Message = string.Empty;
    public Guid SenderID = Guid.Empty;

    public override void Serialize(ref NetworkMessage msg)
    {
        msg.WriteString(Message);
        var hasSender = SenderID != Guid.Empty;
        msg.WriteBoolean(hasSender);
        if (hasSender)
            msg.WriteGuid(SenderID);
    }

    public override void Deserialize(ref NetworkMessage msg)
    {
        Message = msg.ReadString();
        SenderID = msg.ReadBoolean() ? msg.ReadGuid() : Guid.Empty;
    }

    public override void ServerHandler(ref NetworkConnection sender)
    {
        SenderID = NetworkSession.Instance.GuidByConn(ref sender);
        GameSession.Instance.AddChatMessage(SenderID, Message);
        NetworkSession.Instance.SendAll(this, NetworkChannelType.ReliableOrdered);
    }

    public override void ClientHandler()
    {
        if (GameSession.Instance.LocalPlayer.ID == SenderID)
            return;
        GameSession.Instance.AddChatMessage(SenderID, Message);
    }
}
