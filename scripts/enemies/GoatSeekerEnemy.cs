using Godot;
using Tasoloikka2026.Player;

namespace Tasoloikka2026.Enemies;

public partial class GoatSeekerEnemy : EnemyBase
{
    [Export] public float MoveSpeed = 120.0f;
    [Export] public float JumpVelocity = -430.0f;
    [Export] public float JumpCooldown = 0.55f;
    [Export] public float SeekDeadzoneX = 18.0f;

    private float _defaultGravity;
    private float _jumpCooldownTimer;
    private float _stuckTimer;
    private int _facingDirection = 1;
    private Area2D? _damageArea;

    public override void _Ready()
    {
        HitsToKill = 2;
        _defaultGravity = (float)ProjectSettings.GetSetting("physics/2d/default_gravity");
        _damageArea = GetNodeOrNull<Area2D>("DamageArea");
        if (_damageArea != null)
        {
            _damageArea.BodyEntered += OnBodyEntered;
        }

        InitializeEnemyBase("Sprite2D");
    }

    public override void _PhysicsProcess(double delta)
    {
        var dt = (float)delta;
        UpdateEnemyBase(dt);
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
        HandleCommonBodyEntered(body);
    }
}
