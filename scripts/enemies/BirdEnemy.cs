using Godot;
using Tasoloikka2026.Player;
using Tasoloikka2026.Projectiles;

namespace Tasoloikka2026.Enemies;

public partial class BirdEnemy : Area2D
{
    [Export] public float HoverAmplitude = 60.0f;
    [Export] public float HoverSpeed = 1.6f;
    [Export] public int HitsToKill = 2;

    private Vector2 _startPosition;
    private float _hoverTimer;
    private int _remainingHits;
    private AnimatedSprite2D? _sprite;

    public override void _Ready()
    {
        _startPosition = GlobalPosition;
        _remainingHits = HitsToKill;
        _sprite = GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");
        _sprite?.Play("flap");
        BodyEntered += OnBodyEntered;
    }

    public override void _PhysicsProcess(double delta)
    {
        var dt = (float)delta;
        _hoverTimer += dt * HoverSpeed;
        GlobalPosition = _startPosition + new Vector2(0.0f, Mathf.Sin(_hoverTimer) * HoverAmplitude);
    }

    private void OnBodyEntered(Node2D body)
    {
        if (body is StoneProjectile stone)
        {
            stone.QueueFree();
            _remainingHits--;
            if (_remainingHits <= 0)
            {
                QueueFree();
            }

            return;
        }

        if (body is PlayerController player)
        {
            player.Die();
        }
    }
}
