using Godot;
using Tasoloikka2026.Player;

namespace Tasoloikka2026.Objects;

public partial class BouncePad : Area2D
{
    [Export] public float BounceVelocity = -520.0f;

    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
    }

    private void OnBodyEntered(Node2D body)
    {
        if (body is PlayerController player)
        {
            player.Bounce(BounceVelocity);
        }
    }
}
