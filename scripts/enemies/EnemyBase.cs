using Godot;
using Tasoloikka2026.Player;
using Tasoloikka2026.Projectiles;
using Tasoloikka2026.UI;

namespace Tasoloikka2026.Enemies;

public abstract partial class EnemyBase : CharacterBody2D
{
    [Export] public int HitsToKill = 1;
    [Export] public float HitFlashDuration = 0.5f;
    [Export] public Color HitFlashColor = new(0.86f, 0.45f, 0.45f, 1.0f);
    [Export] public Vector2 HealthIndicatorOffset = new(0.0f, -62.0f);
    [Export] public string HitSoundPath = "res://assets/audio/sfx/land.ogg";
    [Export] public float HitSoundVolumeDb = -7.0f;

    protected int RemainingHits { get; private set; }

    private CanvasItem? _visual;
    private Color _baseVisualModulate = Colors.White;
    private float _hitFlashTimer;
    private EnemyHealthIndicator? _healthIndicator;
    private AudioStreamPlayer2D? _hitSfx;
    private PackedScene? _explosionScene;

    protected void InitializeEnemyBase(string visualNodePath)
    {
        HitsToKill = Mathf.Max(1, HitsToKill);
        RemainingHits = HitsToKill;

        _visual = GetNodeOrNull<CanvasItem>(visualNodePath);
        if (_visual != null)
        {
            _baseVisualModulate = _visual.Modulate;
        }

        _healthIndicator = new EnemyHealthIndicator
        {
            Position = HealthIndicatorOffset,
            Visible = false
        };
        _healthIndicator.Configure(HitsToKill);
        AddChild(_healthIndicator);

        var hitSoundStream = GD.Load<AudioStream>(HitSoundPath);
        if (hitSoundStream != null)
        {
            _hitSfx = new AudioStreamPlayer2D
            {
                Stream = hitSoundStream,
                VolumeDb = HitSoundVolumeDb
            };
            AddChild(_hitSfx);
        }

        _explosionScene = GD.Load<PackedScene>("res://scenes/effects/Explosion.tscn");
    }

    protected void UpdateEnemyBase(float dt)
    {
        if (_hitFlashTimer > 0.0f)
        {
            _hitFlashTimer -= dt;
            if (_visual != null)
            {
                _visual.Modulate = HitFlashColor;
            }
        }
        else if (_visual != null && _visual.Modulate != _baseVisualModulate)
        {
            _visual.Modulate = _baseVisualModulate;
        }
    }

    protected bool HandleCommonBodyEntered(Node2D body)
    {
        if (body is PlayerController player)
        {
            player.Die();
            return true;
        }

        if (body is not StoneProjectile stone)
        {
            return false;
        }

        return TryApplyStoneHit(stone);
    }

    public bool TryApplyStoneHit(StoneProjectile? stone = null)
    {
        if (stone != null && stone.IsQueuedForDeletion())
        {
            return false;
        }

        stone?.QueueFree();
        ApplyStoneHit();
        return true;
    }

    private void ApplyStoneHit()
    {
        RemainingHits--;
        _hitFlashTimer = HitFlashDuration;
        _hitSfx?.Play();

        if (_healthIndicator != null && RemainingHits < HitsToKill)
        {
            _healthIndicator.Visible = true;
            _healthIndicator.SetRemaining(RemainingHits);
        }

        if (RemainingHits <= 0)
        {
            SpawnExplosion();
            QueueFree();
        }
    }

    private void SpawnExplosion()
    {
        if (_explosionScene == null)
        {
            return;
        }

        var explosion = _explosionScene.Instantiate<Node2D>();
        explosion.GlobalPosition = GlobalPosition;
        var parentNode = GetTree().CurrentScene ?? GetParent();
        parentNode?.AddChild(explosion);
    }
}
