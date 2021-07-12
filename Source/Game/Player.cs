using System;
using FlaxEngine;
using FlaxEngine.Networking;

public class Player
{
    public Guid ID = Guid.Empty;
    public string Name = string.Empty;
    public Vector3 Position = Vector3.Zero;
    public Quaternion Rotation = Quaternion.Zero;
    
    public Actor Actor = null;
}