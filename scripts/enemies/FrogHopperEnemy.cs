using Godot;
using Tasoloikka2026.Player;
using Tasoloikka2026.Projectiles;

namespace Tasoloikka2026.Enemies;

public partial class FrogHopperEnemy : CharacterBody2D
{
    [Export] public int HitsToKill = 1;
    [Export] public float HopInterval = 1.35f;
    [Export] public float HopHorizontalSpeed = 150.0f;
    [Export] public float HopJumpVelocity = -360.0f;
    [Export] public float SeekDeadzoneX = 18.0f;

    private int _remainingHits;
    private float _defaultGravity;
    private float _hopTimer;
    private int _facingDirection = 1;
    private Area2D? _damageArea;
    private PackedScene? _explosionScene;

    public override void _Ready()
    {
        _remainingHits = HitsToKill;
        _hopTimer = HopInterval * 0.5f;
        _defaultGravity = (float)ProjectSettings.GetSetting("physics/2d/default_gravity");
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
        _hopTimer -= dt;

        if (!IsOnFloor())
        {
            Velocity += new Vector2(0.0f, _defaultGravity * dt);
        }
        else if (_hopTimer <= 0.0f)
        {
            var player = GetTree().GetFirstNodeInGroup("player") as PlayerController;
            if (player != null)
            {
                var dx = player.GlobalPosition.X - GlobalPosition.X;
                if (Mathf.Abs(dx) > SeekDeadzoneX)
                {
                    _facingDirection = dx > 0.0f ? 1 : -1;
                }
            }

            var dir = (float)_facingDirection;
            Velocity = new Vector2(dir * HopHorizontalSpeed, HopJumpVelocity);
            _hopTimer = HopInterval;
        }

        MoveAndSlide();
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
