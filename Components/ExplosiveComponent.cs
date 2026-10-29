using Godot;
using System;

[GlobalClass]
public partial class ExplosiveComponent : DamagingComponent
{
    private PackedScene DebrisScene = GD.Load<PackedScene>("res://Scenes/DebrisScene.tscn");
    
    public override void _Ready()
    {
        bulletParentNode = GetParent<CharacterBody3D>();
        damagingGroup = "allies";
        damage = 70f;
    }

    protected override void Damage(Node3D enemy)
    {
        foreach (var child in enemy.GetChildren())
            {
                if (child is HealthComponent healthComponent)
                {
                    healthComponent.TakeDamage(damage);
                    break;
                }
            }
        GD.Print("Bomb");
        // Эффект взрыва.
        var random = new RandomNumberGenerator();
        random.Randomize();
        for (int i = 0; i < 20; i++)
        {
            var debris = DebrisScene.Instantiate<RigidBody3D>();
            GetTree().CurrentScene.AddChild(debris);
            debris.GlobalPosition = GlobalPosition;
            Vector3 direction = new Vector3(
                random.RandfRange(-1f, 1f),
                random.RandfRange(0.2f, 1f),
                random.RandfRange(-1f, 1f)
            ).Normalized();

            debris.ApplyCentralImpulse(direction * random.RandfRange(3f, 7f));
            debris.ApplyTorqueImpulse(new Vector3(
                random.RandfRange(-2f, 2f),
                random.RandfRange(-2f, 2f),
                random.RandfRange(-2f, 2f)));
        }
        QueueFree();
    }
}
