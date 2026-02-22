using Godot;
using Tasoloikka2026.Player;
using Tasoloikka2026.Projectiles;

namespace Tasoloikka2026.Enemies;

public partial class GoatSeekerEnemy : CharacterBody2D
{
    [Export] public int HitsToKill = 2;
    [Export] public float MoveSpeed = 120.0f;
    [Export] public float JumpVelocity = -430.0f;
    [Export] public float JumpCooldown = 0.55f;
    [Export] public float SeekDeadzoneX = 18.0f;

    private int _remainingHits;
    private float _defaultGravity;
    private float _jumpCooldownTimer;
    private float _stuckTimer;
    private int _facingDirection = 1;
    private Area2D? _damageArea;
    private PackedScene? _explosionScene;

    public override void _Ready()
    {
        _remainingHits = HitsToKill;
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
        _jumpCooldownTimer -= dt;
        var player = GetTree().GetFirstNodeInGroup("player") as PlayerController;
        if (player == null)
        {
            return;
        }

        var dx = player.GlobalPosition.X - GlobalPosition.X;
        if (Mathf.Abs(dx) > SeekDeadzoneX)
        {
            _facingDirection = dx > 0.0f ? 1 : -1;
        }

        var targetDir = _facingDirection;
        Velocity = new Vector2(targetDir * MoveSpeed, Velocity.Y);

        if (!IsOnFloor())
        {
            Velocity += new Vector2(0.0f, _defaultGravity * dt);
        }

        var needJumpForHeight = player.GlobalPosition.Y < GlobalPosition.Y - 26.0f;
        var pushingWall = IsOnFloor() && IsOnWall();
        var notMovingEnough = IsOnFloor() && Mathf.Abs(Velocity.X) > 40.0f && Mathf.Abs(GetRealVelocity().X) < 10.0f;
        _stuckTimer = notMovingEnough ? _stuckTimer + dt : 0.0f;

        if (IsOnFloor() && _jumpCooldownTimer <= 0.0f && (needJumpForHeight || pushingWall || _stuckTimer > 0.25f))
        {
            Velocity = new Vector2(Velocity.X, JumpVelocity);
            _jumpCooldownTimer = JumpCooldown;
            _stuckTimer = 0.0f;
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
