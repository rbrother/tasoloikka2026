using Godot;
using Tasoloikka2026.Enemies;

namespace Tasoloikka2026.Projectiles;

public partial class StoneProjectile : CharacterBody2D
{
    [Export] public float GravityScale = 0.9f;
    [Export] public float LifetimeSeconds = 5.0f;
    [Export] public float DespawnBelowY = 1400.0f;

    private float _defaultGravity;
    private float _lifeTimer;
    private Sprite2D? _sprite;

    public override void _Ready()
    {
        _defaultGravity = (float)ProjectSettings.GetSetting("physics/2d/default_gravity");
        _sprite = GetNodeOrNull<Sprite2D>("Sprite2D");
    }

    public void Launch(Vector2 initialVelocity)
    {
        Velocity = initialVelocity;
    }

    public override void _PhysicsProcess(double delta)
    {
        var dt = (float)delta;

        if (GlobalPosition.Y > DespawnBelowY)
        {
            QueueFree();
            return;
        }

        _lifeTimer += dt;
        if (_lifeTimer >= LifetimeSeconds)
        {
            QueueFree();
            return;
        }

        Velocity += new Vector2(0.0f, _defaultGravity * GravityScale * dt);
        var collision = MoveAndCollide(Velocity * dt);
        if (collision != null)
        {
            if (collision.GetCollider() is EnemyBase enemy && enemy.TryApplyStoneHit(this))
            {
                return;
            }

            QueueFree();
            return;
        }

        if (_sprite != null)
        {
            _sprite.Rotation = Velocity.Angle();
        }
    }
}
