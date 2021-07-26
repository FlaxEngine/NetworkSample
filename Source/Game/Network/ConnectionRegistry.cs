using System;
using System.Collections.Generic;
using System.Linq;
using FlaxEngine.Networking;

public class ConnectionRegistry
{
    private Dictionary<NetworkConnection, Guid> _idByCon = new Dictionary<NetworkConnection, Guid>();
    private Dictionary<Guid, NetworkConnection> _conById = new Dictionary<Guid, NetworkConnection>();

    public void Add(ref NetworkConnection conn, Player player)
    {
        _idByCon.Add(conn, player.ID);
        _conById.Add(player.ID, conn);
    }

    public void Remove(ref NetworkConnection conn)
    {
        var guid = _idByCon[conn];
        _conById.Remove(guid);
        _idByCon.Remove(conn);
    }

    public void Remove(ref Guid guid)
    {
        var conn = _conById[guid];
        _idByCon.Remove(conn);
        _conById.Remove(guid);
    }

    public void Clear()
    {
        _idByCon.Clear();
        _conById.Clear();
    }


    public Guid GuidByConn(ref NetworkConnection conn)
    {
        return _idByCon.ContainsKey(conn) ? _idByCon[conn] : default;
    }

    public NetworkConnection ConnByGuid(ref Guid guid)
    {
        return _conById.ContainsKey(guid) ? _conById[guid] : default;
    }

    public List<NetworkConnection> ToList()
    {
        return _conById.Values.ToList();
    }

    public NetworkConnection[] ToArray()
    {
        return _conById.Values.ToArray();
    }
}
