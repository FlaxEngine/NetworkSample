using System.Collections.Generic;
using FlaxEngine.Networking;

public class PlayerListPacket : NetworkPacket
{
    public List<Player> Players = new List<Player>();

    public override void Serialize(ref NetworkMessage msg)
    {
        msg.WriteInt32(Players.Count + 1);
        for (var i = 0; i < Players.Count; i++)
        {
            msg.WriteString(Players[i].Name);
            msg.WriteGuid(Players[i].ID);
        }

        msg.WriteString(GameSession.Instance.LocalPlayer.Name);
        msg.WriteGuid(GameSession.Instance.LocalPlayer.ID);
    }

    public override void Deserialize(ref NetworkMessage msg)
    {
        var count = msg.ReadInt32();
        Players = new List<Player>();
        for (int i = 0; i < count; i++)
        {
            Player p = new Player();
            p.Name = msg.ReadString();
            p.ID = msg.ReadGuid();
            Players.Add(p);
        }
    }

    public override void ClientHandler()
    {
        for (var i = 0; i < Players.Count; i++)
        {
            if (GameSession.Instance.GetPlayer(Players[i].ID) == null &&
                Players[i].ID != GameSession.Instance.LocalPlayer.ID)
            {
                GameSession.Instance.AddPlayer(Players[i]);
            }
        }
    }
}
