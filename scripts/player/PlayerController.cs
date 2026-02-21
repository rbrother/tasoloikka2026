using Godot;
namespace Tasoloikka2026.Player;

public partial class PlayerController : CharacterBody2D
{
    [Export] public float MoveSpeed = 140.0f;
    [Export] public float JumpVelocity = -260.0f;
    [Export] public float GravityScale = 1.0f;
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

        if (IsOnFloor())
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
        Velocity = new Vector2(direction * MoveSpeed, Velocity.Y);

        if (!IsOnFloor())
        {
            Velocity += new Vector2(0.0f, _defaultGravity * GravityScale * dt);
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
