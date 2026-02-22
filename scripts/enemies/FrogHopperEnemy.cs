using Godot;
using Tasoloikka2026.Player;

namespace Tasoloikka2026.Enemies;

public partial class FrogHopperEnemy : EnemyBase
{
    [Export] public float HopInterval = 1.35f;
    [Export] public float HopHorizontalSpeed = 150.0f;
    [Export] public float HopJumpVelocity = -360.0f;
    [Export] public float SeekDeadzoneX = 18.0f;

    private float _defaultGravity;
    private float _hopTimer;
    private int _facingDirection = 1;
    private Area2D? _damageArea;

    public override void _Ready()
    {
        HitsToKill = 1;
        _hopTimer = HopInterval * 0.5f;
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
        HandleCommonBodyEntered(body);
    }
}
