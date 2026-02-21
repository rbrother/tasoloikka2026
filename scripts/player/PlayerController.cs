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

        if (Input.IsActionJustPressed("ui_accept"))
        {
            _jumpBufferTimer = JumpBufferTime;
        }
        else
        {
            _jumpBufferTimer -= dt;
        }

        var direction = Input.GetAxis("ui_left", "ui_right");
        Velocity = new Vector2(direction * MoveSpeed, Velocity.Y);

        // Apply project gravity for consistent behavior with engine settings.
        var gravity = (float)ProjectSettings.GetSetting("physics/2d/default_gravity");
        if (!IsOnFloor())
        {
            Velocity += new Vector2(0, gravity * GravityScale * dt);
        }

        if (_jumpBufferTimer > 0.0f && _coyoteTimer > 0.0f)
        {
            Velocity = new Vector2(Velocity.X, JumpVelocity);
            _jumpBufferTimer = 0.0f;
            _coyoteTimer = 0.0f;
        }

        MoveAndSlide();
    }
}
