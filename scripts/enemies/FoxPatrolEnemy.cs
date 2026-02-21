using Godot;
using Tasoloikka2026.Player;
using Tasoloikka2026.Projectiles;

namespace Tasoloikka2026.Enemies;

public partial class FoxPatrolEnemy : CharacterBody2D
{
    [Export] public int HitsToKill = 1;
    [Export] public float MoveSpeed = 85.0f;

    private int _remainingHits;
    private float _defaultGravity;
    private int _direction = 1;
    private Area2D? _damageArea;
    private RayCast2D? _floorCheck;
    private PackedScene? _explosionScene;

    public override void _Ready()
    {
        _remainingHits = HitsToKill;
        _defaultGravity = (float)ProjectSettings.GetSetting("physics/2d/default_gravity");
        _damageArea = GetNodeOrNull<Area2D>("DamageArea");
        _floorCheck = GetNodeOrNull<RayCast2D>("FloorCheck");
        if (_damageArea != null)
        {
            _damageArea.BodyEntered += OnBodyEntered;
        }

        _explosionScene = GD.Load<PackedScene>("res://scenes/effects/Explosion.tscn");
    }

    public override void _PhysicsProcess(double delta)
    {
        var dt = (float)delta;
        if (!IsOnFloor())
        {
            Velocity += new Vector2(0.0f, _defaultGravity * dt);
        }

        Velocity = new Vector2(_direction * MoveSpeed, Velocity.Y);
        MoveAndSlide();

        if (_floorCheck != null)
        {
            _floorCheck.TargetPosition = new Vector2(28.0f * _direction, 38.0f);
            _floorCheck.ForceRaycastUpdate();
        }

        if ((IsOnFloor() && IsOnWall()) || (_floorCheck != null && !_floorCheck.IsColliding()))
        {
            _direction *= -1;
        }
    }

    private void OnBodyEntered(Node2D body)
    {
        if (body is PlayerController player)
        {
            player.Die();
            return;
        }

        if (body is StoneProjectile stone)
        {
            stone.QueueFree();
            _remainingHits--;
            if (_remainingHits <= 0)
            {
                SpawnExplosion();
                QueueFree();
            }
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
