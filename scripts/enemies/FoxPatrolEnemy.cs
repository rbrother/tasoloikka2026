using Godot;

namespace Tasoloikka2026.Enemies;

public partial class FoxPatrolEnemy : EnemyBase
{
    [Export] public float MoveSpeed = 85.0f;

    private float _defaultGravity;
    private int _direction = 1;
    private Area2D? _damageArea;
    private RayCast2D? _floorCheck;

    public override void _Ready()
    {
        HitsToKill = 1;
        _defaultGravity = (float)ProjectSettings.GetSetting("physics/2d/default_gravity");
        _damageArea = GetNodeOrNull<Area2D>("DamageArea");
        _floorCheck = GetNodeOrNull<RayCast2D>("FloorCheck");
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
        HandleCommonBodyEntered(body);
    }
}
