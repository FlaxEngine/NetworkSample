using System;
using FlaxEngine;

/// <summary>
/// Player information container (Name, ID, Actor, etc.)
/// </summary>
public class Player
{
    public Guid ID = Guid.Empty;
    public string Name = string.Empty;
    public Vector3 Position = Vector3.Zero;
    public Quaternion Rotation = Quaternion.Zero;

    public Actor Actor = null;
}
