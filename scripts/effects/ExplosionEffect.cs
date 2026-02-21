using Godot;

namespace Tasoloikka2026.Effects;

public partial class ExplosionEffect : Node2D
{
    public override void _Ready()
    {
        var sprite = GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");
        if (sprite != null)
        {
            sprite.AnimationFinished += OnAnimationFinished;
            sprite.Play("explode");
        }
    }

    private void OnAnimationFinished()
    {
        QueueFree();
    }
}
