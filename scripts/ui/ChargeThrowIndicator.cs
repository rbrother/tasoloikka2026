using Godot;
using System.Collections.Generic;

namespace Tasoloikka2026.UI;

public partial class ChargeThrowIndicator : Node2D
{
    [Export] public float Radius = 28.0f;
    [Export] public float RingWidth = 7.0f;
    [Export] public int DashCount = 18;
    [Export(PropertyHint.Range, "0.2,0.95,0.01")] public float DashFillRatio = 0.68f;
    [Export] public Color RingBaseColor = new(0.1f, 0.12f, 0.16f, 0.75f);
    [Export] public Color DashIdleColor = new(0.95f, 0.95f, 0.95f, 0.4f);
    [Export] public Color DashActiveColor = new(1.0f, 0.97f, 0.55f, 1.0f);
    [Export] public Color FillColor = new(1.0f, 0.35f, 0.12f, 0.72f);
    [Export] public Color FillHighlightColor = new(1.0f, 0.8f, 0.25f, 0.92f);

    private float _chargeRatio;

    public void SetCharge(float ratio)
    {
        _chargeRatio = Mathf.Clamp(ratio, 0.0f, 1.0f);
        QueueRedraw();
    }

    public override void _Draw()
    {
        DrawArc(Vector2.Zero, Radius, 0.0f, Mathf.Tau, 96, RingBaseColor, RingWidth, true);

        var dashStep = Mathf.Tau / Mathf.Max(1, DashCount);
        var dashArcLength = dashStep * Mathf.Clamp(DashFillRatio, 0.1f, 0.98f);
        var filledDashCount = Mathf.RoundToInt(_chargeRatio * DashCount);
        for (var i = 0; i < DashCount; i++)
        {
            var start = -Mathf.Pi * 0.5f + i * dashStep;
            var end = start + dashArcLength;
            var color = i < filledDashCount ? DashActiveColor : DashIdleColor;
            DrawArc(Vector2.Zero, Radius, start, end, 10, color, RingWidth, true);
        }

        if (_chargeRatio <= 0.001f)
        {
            return;
        }

        var startAngle = -Mathf.Pi * 0.5f;
        var endAngle = startAngle + Mathf.Tau * _chargeRatio;
        var segments = Mathf.Max(10, Mathf.RoundToInt(64.0f * _chargeRatio));
        var innerRadius = Radius - RingWidth - 4.0f;

        var points = new List<Vector2> { Vector2.Zero };
        for (var i = 0; i <= segments; i++)
        {
            var t = i / (float)segments;
            var angle = Mathf.Lerp(startAngle, endAngle, t);
            points.Add(new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * innerRadius);
        }

        DrawColoredPolygon(points.ToArray(), FillColor);
        DrawArc(Vector2.Zero, innerRadius, startAngle, endAngle, Mathf.Max(12, segments), FillHighlightColor, 4.0f, true);
    }
}
