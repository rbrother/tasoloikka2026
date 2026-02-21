using Godot;
namespace Tasoloikka2026.Player;

public partial class PlayerController : CharacterBody2D
{
    [Export] public float MoveSpeed = 140.0f;
    [Export] public float GroundAcceleration = 1200.0f;
    [Export] public float GroundDeceleration = 1400.0f;
    [Export] public float AirAcceleration = 900.0f;
    [Export] public float AirDeceleration = 700.0f;
    [Export] public float JumpVelocity = -260.0f;
    [Export] public float GravityScale = 1.0f;
    [Export] public float FallGravityMultiplier = 1.2f;
    [Export] public float JumpCutMultiplier = 2.0f;
    [Export] public float MaxFallSpeed = 520.0f;
    [Export] public float CoyoteTime = 0.1f;
    [Export] public float JumpBufferTime = 0.1f;

    private float _coyoteTimer;
    private float _jumpBufferTimer;
    private float _defaultGravity;

    public override void _Ready()
    {
        EnsureInputActions();
        _defaultGravity = (float)ProjectSettings.GetSetting("physics/2d/default_gravity");
    }

    public override void _PhysicsProcess(double delta)
    {
        var dt = (float)delta;
        var isOnFloor = IsOnFloor();

        if (isOnFloor)
        {
            _coyoteTimer = CoyoteTime;
        }
        else
        {
            _coyoteTimer -= dt;
        }

        if (Input.IsActionJustPressed("jump"))
        {
            _jumpBufferTimer = JumpBufferTime;
        }
        else
        {
            _jumpBufferTimer -= dt;
        }

        var direction = Input.GetAxis("move_left", "move_right");
        var targetSpeed = direction * MoveSpeed;
        var acceleration = direction == 0.0f
            ? (isOnFloor ? GroundDeceleration : AirDeceleration)
            : (isOnFloor ? GroundAcceleration : AirAcceleration);
        var newXVelocity = Mathf.MoveToward(Velocity.X, targetSpeed, acceleration * dt);
        Velocity = new Vector2(newXVelocity, Velocity.Y);

        if (!isOnFloor)
        {
            var gravityMultiplier = FallGravityMultiplier;
            if (Velocity.Y < 0.0f && !Input.IsActionPressed("jump"))
            {
                gravityMultiplier = JumpCutMultiplier;
            }

            Velocity += new Vector2(0.0f, _defaultGravity * GravityScale * gravityMultiplier * dt);
            Velocity = new Vector2(Velocity.X, Mathf.Min(Velocity.Y, MaxFallSpeed));
        }

        if (_jumpBufferTimer > 0.0f && _coyoteTimer > 0.0f)
        {
            Velocity = new Vector2(Velocity.X, JumpVelocity);
            _jumpBufferTimer = 0.0f;
            _coyoteTimer = 0.0f;
        }

        MoveAndSlide();
    }

    private static void EnsureInputActions()
    {
        EnsureActionWithKey("move_left", Key.A);
        EnsureActionWithKey("move_right", Key.D);
        EnsureActionWithKey("jump", Key.Space);
    }

    private static void EnsureActionWithKey(string action, Key key)
    {
        if (!InputMap.HasAction(action))
        {
            InputMap.AddAction(action);
        }

        foreach (var existingEvent in InputMap.ActionGetEvents(action))
        {
            if (existingEvent is InputEventKey existingKey && existingKey.PhysicalKeycode == key)
            {
                return;
            }
        }

        var keyEvent = new InputEventKey
        {
            PhysicalKeycode = key
        };
        InputMap.ActionAddEvent(action, keyEvent);
    }
}
