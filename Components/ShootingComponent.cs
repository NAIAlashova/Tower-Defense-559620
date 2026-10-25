using Godot;
using System;

[GlobalClass]
public partial class ShootingComponent : Node
{
    protected CharacterBody3D parentNode;
    [Export]
    protected float Speed {set; get;} = 2f;
    [Export]
    protected float ShootingCooldown {set; get; } = 5f;
    private Node3D Target;
    [Export]
    private VisibleComponent visibleComponent;
    [Export]
    private float CooldownShotting = 1f;
    [Export]
    private PackedScene BulletScene = GD.Load<PackedScene>("res://Scenes/bullet.tscn");
    private float CooldownProgress = 0f;

    public override void _Ready()
    {
      parentNode = GetParent<CharacterBody3D>();  
    }

    public override void _Process(double delta)
    {
        if (IsInstanceValid(Target))
        {
            if (CooldownProgress <= 0.0)
            {
                CooldownProgress = ShootingCooldown;
                ShootBullet();
            }
        }
        else if (visibleComponent != null)
        {
            Target = visibleComponent.GetTarget();
        }
        CooldownProgress -= (float) delta;
    }

    private void ShootBullet()
    {
        Bullet bulletInstance = BulletScene.Instantiate<Bullet>();
        bulletInstance.Position = parentNode.Position;
        bool isAlly = parentNode.IsInGroup("allies");
        if (isAlly)
        {
            bulletInstance.damagingGroup = "enemies";
        }
        else
        {
            bulletInstance.damagingGroup = "allies";
        }
        bulletInstance.parentNode = parentNode;
        if (parentNode is Player)
        {
            bulletInstance.GetDamage(10f);
        }
        else
        {
            bulletInstance.GetDamage(20f);
        }
        parentNode.AddSibling(bulletInstance);
        if (IsInstanceValid(Target))
        {
            bulletInstance.LookAt(Target.Position);
        }
        GD.Print("Bullet");
    }
}
