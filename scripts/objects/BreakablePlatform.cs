using Godot;
using Tasoloikka2026.Player;

namespace Tasoloikka2026.Objects;

public partial class BreakablePlatform : Node2D
{
    [Export] public float BreakDelaySeconds = 1.0f;
    [Export] public float FlashSpeed = 10.0f;

    private CollisionShape2D? _collisionShape;
    private CanvasItem? _visual;
    private Area2D? _trigger;
    private bool _isTriggered;
    private bool _isBroken;
    private float _flashTimer;

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

    public override void _Process(double delta)
    {
        if (_visual == null)
        {
            return;
        }

        if (_isTriggered && !_isBroken)
        {
            _flashTimer += (float)delta * FlashSpeed;
            var t = (Mathf.Sin(_flashTimer) + 1.0f) * 0.5f;
            _visual.Modulate = Color.FromHsv(0.1f, 0.55f - 0.35f * t, 0.70f + 0.30f * t, 1.0f);
            return;
        }

        _visual.Modulate = Colors.White;
    }

    private async void OnTriggerBodyEntered(Node2D body)
    {
        if (_isTriggered || _isBroken)
        {
            return;
        }

        if (body is PlayerController)
        {
            _isTriggered = true;
            await ToSignal(GetTree().CreateTimer(BreakDelaySeconds), SceneTreeTimer.SignalName.Timeout);
            BreakNow();
        }
    }

    private void BreakNow()
    {
        if (_isBroken)
        {
            return;
        }

        _isBroken = true;
        _isTriggered = false;
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
