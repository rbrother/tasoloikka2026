using Godot;
using Tasoloikka2026.Player;
using Tasoloikka2026.Projectiles;

namespace Tasoloikka2026.Enemies;

public partial class MonkeySlingerEnemy : EnemyBase
{
    [Export] public float MoveSpeed = 70.0f;
    [Export] public float JumpVelocity = -320.0f;
    [Export] public float JumpInterval = 2.4f;
    [Export] public float ThrowInterval = 1.5f;
    [Export] public float ThrowSpeed = 580.0f;
    [Export] public float ThrowAngleDegrees = 28.0f;
    [Export] public float SeekDeadzoneX = 18.0f;

    private float _jumpTimer;
    private float _throwTimer;
    private float _defaultGravity;
    private Area2D? _damageArea;
    private PackedScene? _enemyStoneScene;
    private int _facingDirection = -1;

    public override void _Ready()
    {
        HitsToKill = 2;
        _jumpTimer = JumpInterval * 0.5f;
        _throwTimer = ThrowInterval * 0.7f;
        _defaultGravity = (float)ProjectSettings.GetSetting("physics/2d/default_gravity");
        _damageArea = GetNodeOrNull<Area2D>("DamageArea");
        if (_damageArea != null)
        {
            _damageArea.BodyEntered += OnBodyEntered;
        }

        _enemyStoneScene = GD.Load<PackedScene>("res://scenes/projectiles/EnemyStone.tscn");
        InitializeEnemyBase("Sprite2D");
    }

    public override void _PhysicsProcess(double delta)
    {
        var dt = (float)delta;
        UpdateEnemyBase(dt);
        var player = GetTree().GetFirstNodeInGroup("player") as PlayerController;

        _throwTimer -= dt;
        _jumpTimer -= dt;

        var moveDir = 0.0f;
        if (player != null)
        {
            var dx = player.GlobalPosition.X - GlobalPosition.X;
            if (Mathf.Abs(dx) > SeekDeadzoneX)
            {
                _facingDirection = dx > 0.0f ? 1 : -1;
            }

            moveDir = _facingDirection;
        }

        Velocity = new Vector2(moveDir * MoveSpeed, Velocity.Y);
        if (!IsOnFloor())
        {
            Velocity += new Vector2(0.0f, _defaultGravity * dt);
        }
        else if (_jumpTimer <= 0.0f)
        {
            Velocity = new Vector2(Velocity.X, JumpVelocity);
            _jumpTimer = JumpInterval;
        }

        if (player != null && _throwTimer <= 0.0f)
        {
            ThrowAt(player);
            _throwTimer = ThrowInterval;
        }

        MoveAndSlide();
    }

    private void ThrowAt(PlayerController player)
    {
        if (_enemyStoneScene == null)
        {
            return;
        }

        var stone = _enemyStoneScene.Instantiate<EnemyStoneProjectile>();
        var directionX = (float)_facingDirection;

        stone.GlobalPosition = GlobalPosition + new Vector2(20.0f * directionX, -24.0f);
        var angle = Mathf.DegToRad(ThrowAngleDegrees);
        var v = new Vector2(directionX * Mathf.Cos(angle), -Mathf.Sin(angle)) * ThrowSpeed;
        stone.Launch(v);
        var parentNode = GetTree().CurrentScene ?? GetParent();
        parentNode?.AddChild(stone);
    }

    private void OnBodyEntered(Node2D body)
    {
        HandleCommonBodyEntered(body);
    }
}
