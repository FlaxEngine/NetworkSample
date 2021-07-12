using FlaxEngine.Networking;

public class PlayerDisconnectPacket : NetworkPacket
{
    public override void Serialize(ref NetworkMessage msg)
    {
        
    }

    public override void Deserialize(ref NetworkMessage msg)
    {
       
    }

    public override void ServerHandler(ref NetworkConnection sender)
    {
        var g = NetworkSession.Instance.GuidByConn(ref sender);
        NetworkSession.Instance.DisconnectPlayer(ref g);
    }
}