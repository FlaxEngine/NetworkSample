using System;
using FlaxEngine;
using FlaxEngine.Json;
using FlaxEngine.Networking;

public class ConnectionResponsePacket : NetworkPacket
{
    public enum ConnectionState : byte
    {
        Accepted,
        Rejected,
    }

    public ConnectionState State;
    public Guid ID = Guid.Empty;

    public override void Serialize(ref NetworkMessage msg)
    {
        msg.WriteByte((byte)State);
        var bytes = ID.ToByteArray();
        msg.WriteInt32(bytes.Length);
        msg.WriteBytes(bytes, bytes.Length);
    }

    public override void Deserialize(ref NetworkMessage msg)
    {
        State = (ConnectionState)msg.ReadByte();
        var length = msg.ReadInt32();
        byte[] bytes = new byte[length];
        msg.ReadBytes(bytes, length);
        ID = new Guid(bytes);
    }

    public override void ClientHandler()
    {
        if (State == ConnectionState.Accepted)
        {
            GameSession.Instance.LocalPlayer.ID = ID;
            JsonSerializer.ParseID("74a68a984824b4510d12589f199ad68f", out var guid);
            Debug.Log("Connection accepted !");
            Level.ChangeSceneAsync(guid);
        }
        else
        {
            Debug.Log("Connection rejected !");
        }
    }
}
