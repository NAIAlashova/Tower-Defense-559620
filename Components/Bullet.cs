using Godot;
using System;
using System.ComponentModel;

[GlobalClass]
public partial class Bullet : Area3D
{
    [Export]
    private float BulletSpeed = 20f;
    public string damagingGroup;
    public Node3D parentNode;

    public override void _Process(double delta)
    {
        Position += -Transform.Basis.Z*BulletSpeed*(float)delta;
    }

    public void _on_body_entered(Node3D body)
    {
        if (body.IsInGroup(damagingGroup))
        {
            GD.Print("in enemie");
            foreach (var child in body.GetChildren())
            {
                if (child is HealthComponent healthComponent)
                {
                    healthComponent.TakeDamage(30);
                    break;
                }
            }
            QueueFree();
        }
    }
}
