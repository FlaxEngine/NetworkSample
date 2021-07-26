using System;
using System.Collections.Generic;
using FlaxEngine;
using FlaxEngine.Networking;

public class PlayersTransformPacket : NetworkPacket
{
    public struct TransformEntry
    {
        public Guid Guid;
        public Vector3 Position;
        public Quaternion Rotation;
    }

    public List<TransformEntry> Transforms = new List<TransformEntry>();

    public override void Serialize(ref NetworkMessage msg)
    {
        msg.WriteInt32(Transforms.Count);
        for (var i = 0; i < Transforms.Count; i++)
        {
            msg.WriteGuid(Transforms[i].Guid);
            msg.WriteVector3(Transforms[i].Position);
            msg.WriteQuaternion(Transforms[i].Rotation);
        }
    }

    public override void Deserialize(ref NetworkMessage msg)
    {
        Transforms.Clear();
        var count = msg.ReadInt32();
        for (int i = 0; i < count; i++)
        {
            TransformEntry te = new TransformEntry();
            te.Guid = msg.ReadGuid();
            te.Position = msg.ReadVector3();
            te.Rotation = msg.ReadQuaternion();
            Transforms.Add(te);
        }
    }

    public override void ClientHandler()
    {
        for (var i = 0; i < Transforms.Count; i++)
        {
            if (GameSession.Instance.LocalPlayer.ID == Transforms[i].Guid)
                continue;
            var player = GameSession.Instance.GetPlayer(Transforms[i].Guid);
            player.Position = Transforms[i].Position;
            player.Rotation = Transforms[i].Rotation;
        }
    }
}
