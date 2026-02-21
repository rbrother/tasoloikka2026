using Godot;

namespace Tasoloikka2026.UI;

public partial class HudController : CanvasLayer
{
    [Export] public string StartText = "A/D to move, Space to jump";

    public override void _Ready()
    {
        var label = GetNodeOrNull<Label>("DebugLabel");
        if (label != null)
        {
            label.Text = StartText;
        }
    }
}
