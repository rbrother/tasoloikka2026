using Godot;

namespace Tasoloikka2026.Objects;

public partial class BreakablePlatform : Node2D
{
    [Export] public float BreakDelaySeconds = 1.0f;

    private CollisionShape2D? _collisionShape;
    private CanvasItem? _visual;
    private Area2D? _trigger;
    private bool _isTriggered;
    private float _timer;

    public override void _Ready()
    {
        _collisionShape = GetNodeOrNull<CollisionShape2D>("PlatformBody/CollisionShape2D");
        _visual = GetNodeOrNull<CanvasItem>("PlatformBody/Visual");
        _trigger = GetNodeOrNull<Area2D>("TriggerArea");
        if (_trigger != null)
        {
            _trigger.BodyEntered += OnTriggerBodyEntered;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!_isTriggered)
        {
            return;
        }

        _timer += (float)delta;
        if (_timer >= BreakDelaySeconds)
        {
            BreakNow();
        }
    }

    private void OnTriggerBodyEntered(Node2D body)
    {
        if (_isTriggered)
        {
            return;
        }

        if (body is CharacterBody2D)
        {
            _isTriggered = true;
            _timer = 0.0f;
        }
    }

    private void BreakNow()
    {
        _isTriggered = false;
        SetPhysicsProcess(false);
        if (_collisionShape != null)
        {
            _collisionShape.Disabled = true;
        }

        if (_visual != null)
        {
            _visual.Visible = false;
        }

        if (_trigger != null)
        {
            _trigger.Monitoring = false;
        }
    }
}
