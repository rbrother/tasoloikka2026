using Godot;
using Tasoloikka2026.Player;
using Tasoloikka2026.Projectiles;

namespace Tasoloikka2026.Enemies;

public partial class BirdEnemy : CharacterBody2D
{
    [Export] public float HoverAmplitude = 60.0f;
    [Export] public float HoverSpeed = 1.6f;
    [Export] public float MaxVerticalSpeed = 240.0f;
    [Export] public int HitsToKill = 2;

    private Vector2 _startPosition;
    private float _hoverTimer;
    private int _remainingHits;
    private AnimatedSprite2D? _sprite;
    private Area2D? _damageArea;
    private PackedScene? _explosionScene;

    public override void _Ready()
    {
        _startPosition = GlobalPosition;
        _remainingHits = HitsToKill;
        _sprite = GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");
        _sprite?.Play("flap");
        _damageArea = GetNodeOrNull<Area2D>("DamageArea");
        if (_damageArea != null)
        {
            _damageArea.BodyEntered += OnBodyEntered;
        }

        _explosionScene = GD.Load<PackedScene>("res://scenes/effects/Explosion.tscn");
    }

    public override void _PhysicsProcess(double delta)
    {
        var dt = (float)delta;
        _hoverTimer += dt * HoverSpeed;
        var targetY = _startPosition.Y + Mathf.Sin(_hoverTimer) * HoverAmplitude;
        var targetVelocityY = dt > 0.0f ? (targetY - GlobalPosition.Y) / dt : 0.0f;
        targetVelocityY = Mathf.Clamp(targetVelocityY, -MaxVerticalSpeed, MaxVerticalSpeed);
        Velocity = new Vector2(0.0f, targetVelocityY);
        MoveAndSlide();
    }

    private void OnBodyEntered(Node2D body)
    {
        if (body is StoneProjectile stone)
        {
            stone.QueueFree();
            _remainingHits--;
            if (_remainingHits <= 0)
            {
                SpawnExplosion();
                QueueFree();
            }

            return;
        }

        if (body is PlayerController player)
        {
            player.Die();
        }
    }

    private void SpawnExplosion()
    {
        if (_explosionScene == null)
        {
            return;
        }

        var explosion = _explosionScene.Instantiate<Node2D>();
        explosion.GlobalPosition = GlobalPosition;
        var parentNode = GetTree().CurrentScene ?? GetParent();
        parentNode?.AddChild(explosion);
    }
}
