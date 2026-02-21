using Godot;
using Tasoloikka2026.Player;

namespace Tasoloikka2026.Objects;

public partial class FinishDoor : Area2D
{
    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
    }

    private void OnBodyEntered(Node2D body)
    {
        if (body is PlayerController player)
        {
            player.Win();
        }
    }
}
