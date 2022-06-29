using System;
using FlaxEngine;
using FlaxEngine.Networking;

public class LocalPlayerScript : Script
{
    public CharacterController PlayerController;
    public Camera Camera;

    public float CameraSmoothing = 20.0f;

    public bool CanJump = true;
    public bool UseMouse = true;
    public float JumpForce = 800;

    public float Friction = 8.0f;
    public float GroundAccelerate = 5000;
    public float AirAccelerate = 10000;
    public float MaxVelocityGround = 400;
    public float MaxVelocityAir = 200;

    private Vector3 _velocity;
    private bool _jump;
    private float _pitch;
    private float _yaw;
    private float _horizontal;
    private float _vertical;
    private float _lastTransformSent;
    private Actor _chatActor;

    /// <summary>
    /// Adds the movement and rotation to the camera (as input).
    /// </summary>
    /// <param name="horizontal">The horizontal input.</param>
    /// <param name="vertical">The vertical input.</param>
    /// <param name="pitch">The pitch rotation input.</param>
    /// <param name="yaw">The yaw rotation input.</param>
    public void AddMovementRotation(float horizontal, float vertical, float pitch, float yaw)
    {
        _pitch += pitch;
        _yaw += yaw;
        _horizontal += horizontal;
        _vertical += vertical;
    }

    public override void OnEnable()
    {
        _lastTransformSent = 0;
        Actor.Transform = Scene.FindActor("SpawnPoint").Transform;
        Actor.Name = "Player_" + GameSession.Instance.LocalPlayer.Name;
        _chatActor = Scene.FindActor("Chat");
    }

    public override void OnUpdate()
    {
        if (UseMouse)
        {
            Screen.CursorVisible = false;
            Screen.CursorLock = CursorLockMode.Locked;

            var mouseDelta = new Float2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            _pitch = Mathf.Clamp(_pitch + mouseDelta.Y, -88, 88);
            _yaw += mouseDelta.X;
        }

        if (CanJump && Input.GetAction("Jump"))
            _jump = true;

        GameSession.Instance.LocalPlayer.Position = Actor.Transform.Translation;
        GameSession.Instance.LocalPlayer.Rotation = Actor.Transform.Orientation;
        if (Time.UnscaledGameTime - _lastTransformSent > 0.05f)
        {
            PlayerTransformPacket ptp = new PlayerTransformPacket();
            ptp.Position = Actor.Transform.Translation;
            ptp.Rotation = Actor.Transform.Orientation;
            NetworkSession.Instance.Send(ptp, NetworkChannelType.UnreliableOrdered);
            _lastTransformSent = Time.UnscaledGameTime;
        }

        if (Transform.Translation.Y < -5000)
        {
            Actor.Position = Scene.FindActor("SpawnPoint").Transform.Translation;
        }
    }

    private Vector3 Horizontal(Vector3 v)
    {
        return new Vector3(v.X, 0, v.Z);
    }

    public override void OnFixedUpdate()
    {
        var camFactor = Mathf.Saturate(CameraSmoothing * Time.DeltaTime);
        Camera.LocalOrientation = Quaternion.Lerp(Camera.LocalOrientation, Quaternion.Euler(_pitch, 0, 0), camFactor);
        PlayerController.Orientation = Quaternion.Lerp(PlayerController.LocalOrientation, Quaternion.Euler(0, _yaw, 0), camFactor);

        var inputH = Input.GetAxis("Horizontal") + _horizontal;
        var inputV = Input.GetAxis("Vertical") + _vertical;
        _horizontal = 0;
        _vertical = 0;

        var velocity = new Vector3(inputH, 0.0f, inputV);
        velocity.Normalize();
        velocity = Actor.Transform.TransformDirection(velocity);

        if (PlayerController.IsGrounded)
        {
            velocity = MoveGround(velocity.Normalized, Horizontal(_velocity));
            velocity.Y = -Mathf.Abs(Physics.Gravity.Y * 0.5f);
        }
        else
        {
            velocity = MoveAir(velocity.Normalized, Horizontal(_velocity));
            velocity.Y = _velocity.Y;
        }

        if (velocity.Length < 0.05f)
            velocity = Vector3.Zero;

        if (_jump && PlayerController.IsGrounded)
            velocity.Y = JumpForce;

        _jump = false;

        velocity.Y += -Mathf.Abs(Physics.Gravity.Y * 2.5f) * Time.DeltaTime;

        if ((PlayerController.Flags & CharacterController.CollisionFlags.Above) != 0)
        {
            if (velocity.Y > 0)
                velocity.Y = 0;
        }

        if (_chatActor != null && !_chatActor.GetScript<ChatScript>().IsWriting)
            PlayerController.Move(velocity * Time.DeltaTime);
        _velocity = velocity;
    }

    // accelDir: normalized direction that the player has requested to move (taking into account the movement keys and look direction)
    // prevVelocity: The current velocity of the player, before any additional calculations
    // accelerate: The server-defined player acceleration value
    // maxVelocity: The server-defined maximum player velocity (this is not strictly adhered to due to strafejumping)
    private Vector3 Accelerate(Vector3 accelDir, Vector3 prevVelocity, float accelerate, float maxVelocity)
    {
        float projVel = (float)Vector3.Dot(prevVelocity, accelDir);
        float accelVel = accelerate * Time.DeltaTime;
        if (projVel + accelVel > maxVelocity)
            accelVel = maxVelocity - projVel;

        return prevVelocity + accelDir * accelVel;
    }

    private Vector3 MoveGround(Vector3 accelDir, Vector3 prevVelocity)
    {
        var speed = prevVelocity.Length;
        if (Math.Abs(speed) > 0.01f)
        {
            var drop = speed * Friction * Time.DeltaTime;
            prevVelocity *= Mathf.Max(speed - drop, 0) / speed;
        }

        return Accelerate(accelDir, prevVelocity, GroundAccelerate, MaxVelocityGround);
    }

    private Vector3 MoveAir(Vector3 accelDir, Vector3 prevVelocity)
    {
        return Accelerate(accelDir, prevVelocity, AirAccelerate, MaxVelocityAir);
    }

    public override void OnDebugDraw()
    {
        var trans = PlayerController.Transform;
        DebugDraw.DrawWireTube(trans.Translation, trans.Orientation * Quaternion.Euler(90, 0, 0), PlayerController.Radius, PlayerController.Height, Color.Blue);
    }
}
