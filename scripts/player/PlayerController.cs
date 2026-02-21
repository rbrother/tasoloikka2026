using Godot;
namespace Tasoloikka2026.Player;

public partial class PlayerController : CharacterBody2D
{
    [Export] public float MoveSpeed = 130.0f;
    [Export] public float GroundAcceleration = 900.0f;
    [Export] public float GroundDeceleration = 1000.0f;
    [Export] public float AirAcceleration = 700.0f;
    [Export] public float AirDeceleration = 500.0f;
    [Export] public float JumpVelocity = -360.0f;
    [Export] public float GravityScale = 0.62f;
    [Export] public float FallGravityMultiplier = 1.05f;
    [Export] public float JumpCutMultiplier = 1.35f;
    [Export] public float MaxFallSpeed = 330.0f;
    [Export] public float CoyoteTime = 0.12f;
    [Export] public float JumpBufferTime = 0.12f;

    private float _coyoteTimer;
    private float _jumpBufferTimer;
    private float _defaultGravity;
    private AnimatedSprite2D? _animatedSprite;
    private int _facing = 1;

    public override void _Ready()
    {
        EnsureInputActions();
        _defaultGravity = (float)ProjectSettings.GetSetting("physics/2d/default_gravity");
        _animatedSprite = GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");
        _animatedSprite?.Play("idle");
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

        UpdateAnimation();
        MoveAndSlide();
    }

    private void UpdateAnimation()
    {
        if (_animatedSprite == null)
        {
            return;
        }

        if (Velocity.X > 2.0f)
        {
            _facing = 1;
        }
        else if (Velocity.X < -2.0f)
        {
            _facing = -1;
        }

        // Art is authored facing right, flip horizontally when moving left.
        _animatedSprite.FlipH = _facing < 0;

        var shouldWalk = Mathf.Abs(Velocity.X) > 5.0f && IsOnFloor();
        var targetAnimation = shouldWalk ? "walk" : "idle";
        if (_animatedSprite.Animation != targetAnimation)
        {
            _animatedSprite.Play(targetAnimation);
        }
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
