using System;
using FlaxEngine.Networking;

public class PlayerConnectedPacket : NetworkPacket
{
    public Guid ID;
    public string Username;

    public override void Serialize(ref NetworkMessage msg)
    {
        msg.WriteGuid(ID);
        msg.WriteString(Username);
    }

    public override void Deserialize(ref NetworkMessage msg)
    {
        ID = msg.ReadGuid();
        Username = msg.ReadString();
    }

    public override void ClientHandler()
    {
        if (ID == GameSession.Instance.LocalPlayer.ID)
            return;
        GameSession.Instance.AddPlayer(ref ID, Username);
    }
}
