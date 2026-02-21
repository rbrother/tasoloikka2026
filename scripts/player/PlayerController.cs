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

        // CharacterBody2D gravity comes from project/default physics settings.
        var gravity = GetGravity();
        if (!IsOnFloor())
        {
            Velocity += gravity * GravityScale * dt;
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
