using Godot;
using Tasoloikka2026.Projectiles;

namespace Tasoloikka2026.Player;

public partial class PlayerController : CharacterBody2D
{
    [Export] public float MoveSpeed = 130.0f;
    [Export] public float GroundAcceleration = 900.0f;
    [Export] public float GroundDeceleration = 1000.0f;
    [Export] public float AirAcceleration = 700.0f;
    [Export] public float AirDeceleration = 500.0f;
    [Export] public float JumpVelocity = -360.0f;
    [Export] public float GravityScale = 0.62f;
    [Export] public float FallGravityMultiplier = 1.05f;
    [Export] public float JumpCutMultiplier = 1.35f;
    [Export] public float MaxFallSpeed = 330.0f;
    [Export] public float CoyoteTime = 0.12f;
    [Export] public float JumpBufferTime = 0.12f;

    [Export] public float MinThrowSpeed = 260.0f;
    [Export] public float MaxThrowSpeed = 1240.0f;
    [Export] public float MaxThrowChargeTime = 1.0f;
    [Export] public float ThrowAngleDegrees = 35.0f;
    [Export] public Vector2 ThrowSpawnOffset = new(26.0f, -34.0f);

    private float _coyoteTimer;
    private float _jumpBufferTimer;
    private float _defaultGravity;
    private AnimatedSprite2D? _animatedSprite;
    private PackedScene? _stoneScene;
    private Node2D? _chargeIndicator;
    private Polygon2D? _chargeFill;
    private int _facing = 1;
    private bool _isChargingThrow;
    private float _throwChargeTimer;

    public override void _Ready()
    {
        EnsureInputActions();
        _defaultGravity = (float)ProjectSettings.GetSetting("physics/2d/default_gravity");
        _animatedSprite = GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");
        _animatedSprite?.Play("idle");
        _stoneScene = GD.Load<PackedScene>("res://scenes/projectiles/Stone.tscn");
        _chargeIndicator = GetNodeOrNull<Node2D>("ChargeIndicator");
        _chargeFill = GetNodeOrNull<Polygon2D>("ChargeIndicator/Fill");
        if (_chargeIndicator != null)
        {
            _chargeIndicator.Visible = false;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        var dt = (float)delta;
        var isOnFloor = IsOnFloor();

        HandleThrowInput(dt);

        if (isOnFloor)
        {
            _coyoteTimer = CoyoteTime;
        }
        else
        {
            _coyoteTimer -= dt;
        }

        if (Input.IsActionJustPressed("jump"))
        {
            _jumpBufferTimer = JumpBufferTime;
        }
        else
        {
            _jumpBufferTimer -= dt;
        }

        var direction = Input.GetAxis("move_left", "move_right");
        var targetSpeed = direction * MoveSpeed;
        var acceleration = direction == 0.0f
            ? (isOnFloor ? GroundDeceleration : AirDeceleration)
            : (isOnFloor ? GroundAcceleration : AirAcceleration);
        var newXVelocity = Mathf.MoveToward(Velocity.X, targetSpeed, acceleration * dt);
        Velocity = new Vector2(newXVelocity, Velocity.Y);

        if (!isOnFloor)
        {
            var gravityMultiplier = FallGravityMultiplier;
            if (Velocity.Y < 0.0f && !Input.IsActionPressed("jump"))
            {
                gravityMultiplier = JumpCutMultiplier;
            }

            Velocity += new Vector2(0.0f, _defaultGravity * GravityScale * gravityMultiplier * dt);
            Velocity = new Vector2(Velocity.X, Mathf.Min(Velocity.Y, MaxFallSpeed));
        }

        if (_jumpBufferTimer > 0.0f && _coyoteTimer > 0.0f)
        {
            Velocity = new Vector2(Velocity.X, JumpVelocity);
            _jumpBufferTimer = 0.0f;
            _coyoteTimer = 0.0f;
        }

        UpdateAnimation();
        UpdateChargeIndicator();
        MoveAndSlide();
    }

    private void HandleThrowInput(float dt)
    {
        if (Input.IsActionJustPressed("throw"))
        {
            _isChargingThrow = true;
            _throwChargeTimer = 0.0f;
        }

        if (_isChargingThrow && Input.IsActionPressed("throw"))
        {
            _throwChargeTimer = Mathf.Min(_throwChargeTimer + dt, MaxThrowChargeTime);
        }

        if (_isChargingThrow && Input.IsActionJustReleased("throw"))
        {
            ThrowStone();
            _isChargingThrow = false;
            _throwChargeTimer = 0.0f;
        }
    }

    private void ThrowStone()
    {
        if (_stoneScene == null)
        {
            return;
        }

        var stone = _stoneScene.Instantiate<StoneProjectile>();
        var spawnOffset = new Vector2(Mathf.Abs(ThrowSpawnOffset.X) * _facing, ThrowSpawnOffset.Y);
        stone.GlobalPosition = GlobalPosition + spawnOffset;

        var chargeRatio = GetChargeRatio();
        var speed = Mathf.Lerp(MinThrowSpeed, MaxThrowSpeed, chargeRatio);
        var throwAngle = Mathf.DegToRad(ThrowAngleDegrees);
        var throwDirection = new Vector2(_facing * Mathf.Cos(throwAngle), -Mathf.Sin(throwAngle)).Normalized();
        var inheritedVelocity = new Vector2(Velocity.X * 0.35f, 0.0f);
        stone.Launch(throwDirection * speed + inheritedVelocity);

        var parentNode = GetTree().CurrentScene ?? GetParent();
        parentNode?.AddChild(stone);
    }

    private float GetChargeRatio()
    {
        if (MaxThrowChargeTime <= 0.0f)
        {
            return 1.0f;
        }

        return Mathf.Clamp(_throwChargeTimer / MaxThrowChargeTime, 0.0f, 1.0f);
    }

    private void UpdateChargeIndicator()
    {
        if (_chargeIndicator == null || _chargeFill == null)
        {
            return;
        }

        if (!_isChargingThrow)
        {
            _chargeIndicator.Visible = false;
            return;
        }

        _chargeIndicator.Visible = true;
        _chargeIndicator.Scale = _facing > 0 ? Vector2.One : new Vector2(-1.0f, 1.0f);
        _chargeIndicator.Position = _facing > 0
            ? new Vector2(26.0f, -60.0f)
            : new Vector2(-26.0f, -60.0f);

        var ratio = GetChargeRatio();
        _chargeFill.Scale = new Vector2(Mathf.Max(0.02f, ratio), 1.0f);
    }

    private void UpdateAnimation()
    {
        if (_animatedSprite == null)
        {
            return;
        }

        if (Velocity.X > 2.0f)
        {
            _facing = 1;
        }
        else if (Velocity.X < -2.0f)
        {
            _facing = -1;
        }

        _animatedSprite.FlipH = _facing < 0;

        var shouldWalk = Mathf.Abs(Velocity.X) > 5.0f && IsOnFloor();
        var targetAnimation = shouldWalk ? "walk" : "idle";
        if (_animatedSprite.Animation != targetAnimation)
        {
            _animatedSprite.Play(targetAnimation);
        }
    }

    private static void EnsureInputActions()
    {
        EnsureActionWithKey("move_left", Key.A);
        EnsureActionWithKey("move_right", Key.D);
        EnsureActionWithKey("jump", Key.Space);
        EnsureActionWithKey("throw", Key.Comma);
    }

    private static void EnsureActionWithKey(string action, Key key)
    {
        if (!InputMap.HasAction(action))
        {
            InputMap.AddAction(action);
        }

        foreach (var existingEvent in InputMap.ActionGetEvents(action))
        {
            if (existingEvent is InputEventKey existingKey && existingKey.PhysicalKeycode == key)
            {
                return;
            }
        }

        var keyEvent = new InputEventKey
        {
            PhysicalKeycode = key
        };
        InputMap.ActionAddEvent(action, keyEvent);
    }
}
