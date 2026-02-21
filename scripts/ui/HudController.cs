using Godot;

namespace Tasoloikka2026.UI;

public partial class HudController : CanvasLayer
{
    [Export] public string StartText = "A/D move, Space jump, S+Space drop, hold Comma to charge and throw";

    public override void _Ready()
    {
        var label = GetNodeOrNull<Label>("DebugLabel");
        if (label != null)
        {
            label.Text = StartText;
        }
    }
}
