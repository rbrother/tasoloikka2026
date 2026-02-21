using Godot;

namespace Tasoloikka2026.Core;

public partial class GameManager : Node2D
{
    private Label? _debugLabel;
    private CharacterBody2D? _player;

    public override void _Ready()
    {
        _debugLabel = GetNodeOrNull<Label>("HUD/DebugLabel");
        _player = GetNodeOrNull<CharacterBody2D>("Player");
    }

    public override void _Process(double delta)
    {
        if (_debugLabel == null || _player == null)
        {
            return;
        }

        _debugLabel.Text = $"Pos: {_player.GlobalPosition}  Vel: {_player.Velocity}";
    }
}
