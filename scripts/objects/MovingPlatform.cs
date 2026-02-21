using Godot;

namespace Tasoloikka2026.Objects;

public partial class MovingPlatform : AnimatableBody2D
{
    [Export] public Vector2 MoveOffset = new(220.0f, 0.0f);
    [Export] public float MoveSpeed = 1.0f;

    private Vector2 _startPosition;
    private float _t;

    public override void _Ready()
    {
        _startPosition = GlobalPosition;
    }

    public override void _PhysicsProcess(double delta)
    {
        _t += (float)delta * MoveSpeed;
        GlobalPosition = _startPosition + MoveOffset * Mathf.Sin(_t);
    }
}
