using Godot;

namespace Tasoloikka2026.Enemies;

public partial class BirdEnemy : EnemyBase
{
    [Export] public float HoverAmplitude = 60.0f;
    [Export] public float HoverSpeed = 1.6f;
    [Export] public float MaxVerticalSpeed = 240.0f;

    private Vector2 _startPosition;
    private float _hoverTimer;
    private AnimatedSprite2D? _sprite;
    private Area2D? _damageArea;

    public override void _Ready()
    {
        _startPosition = GlobalPosition;
        HitsToKill = 2;
        _sprite = GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");
        _sprite?.Play("flap");
        _damageArea = GetNodeOrNull<Area2D>("DamageArea");
        if (_damageArea != null)
        {
            _damageArea.BodyEntered += OnBodyEntered;
        }

        InitializeEnemyBase("AnimatedSprite2D");
    }

    public override void _PhysicsProcess(double delta)
    {
        var dt = (float)delta;
        UpdateEnemyBase(dt);
        _hoverTimer += dt * HoverSpeed;
        var targetY = _startPosition.Y + Mathf.Sin(_hoverTimer) * HoverAmplitude;
        var targetVelocityY = dt > 0.0f ? (targetY - GlobalPosition.Y) / dt : 0.0f;
        targetVelocityY = Mathf.Clamp(targetVelocityY, -MaxVerticalSpeed, MaxVerticalSpeed);
        Velocity = new Vector2(0.0f, targetVelocityY);
        MoveAndSlide();
    }

    private void OnBodyEntered(Node2D body)
    {
        HandleCommonBodyEntered(body);
    }
}
