using Godot;

namespace Tasoloikka2026.UI;

public partial class EnemyHealthIndicator : Node2D
{
    [Export] public float Radius = 20.0f;
    [Export] public float RingWidth = 6.0f;
    [Export] public float GapRatio = 0.23f;
    [Export] public Color ActiveColor = new(1.0f, 0.84f, 0.32f, 1.0f);
    [Export] public Color InactiveColor = new(0.24f, 0.18f, 0.18f, 0.7f);
    [Export] public Color BackColor = new(0.08f, 0.08f, 0.1f, 0.55f);

    private int _maxSegments = 1;
    private int _remainingSegments = 1;

    public void Configure(int maxSegments)
    {
        _maxSegments = Mathf.Max(1, maxSegments);
        _remainingSegments = _maxSegments;
        QueueRedraw();
    }

    public void SetRemaining(int remainingSegments)
    {
        _remainingSegments = Mathf.Clamp(remainingSegments, 0, _maxSegments);
        QueueRedraw();
    }

    public override void _Draw()
    {
        DrawArc(Vector2.Zero, Radius, 0.0f, Mathf.Tau, 96, BackColor, RingWidth + 3.0f, true);

        var step = Mathf.Tau / _maxSegments;
        var gap = step * Mathf.Clamp(GapRatio, 0.02f, 0.6f);
        for (var i = 0; i < _maxSegments; i++)
        {
            var start = -Mathf.Pi * 0.5f + i * step + gap * 0.5f;
            var end = start + step - gap;
            var color = i < _remainingSegments ? ActiveColor : InactiveColor;
            DrawArc(Vector2.Zero, Radius, start, end, 18, color, RingWidth, true);
        }
    }
}
