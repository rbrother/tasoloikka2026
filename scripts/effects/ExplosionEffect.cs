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

        var stream = GD.Load<AudioStream>("res://assets/audio/sfx/explosion.ogg");
        if (stream != null)
        {
            var sfx = new AudioStreamPlayer2D
            {
                Stream = stream,
                VolumeDb = -2.0f
            };
            AddChild(sfx);
            sfx.Play();
        }
    }

    private void OnAnimationFinished()
    {
        QueueFree();
    }
}
