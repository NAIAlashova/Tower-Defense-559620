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
    private DamagingComponent damagingComponent;

// Задаем значения переменным компонента DamagingComponent.
    public override void _Ready()
    {
        foreach (var child in GetChildren())
        {
            if (child is DamagingComponent _damagingComponent)
            {
                damagingComponent = _damagingComponent;
                break;
            }

        }
        damagingComponent.damagingGroup = damagingGroup;
        damagingComponent.shootedNode = parentNode;
    }

    public override void _Process(double delta)
    {
        Position += -Transform.Basis.Z*BulletSpeed*(float)delta;
    }

    public void _on_body_entered(Node3D body)
    {
        if (body.IsInGroup(damagingGroup))
        {
            damagingComponent.Hit(body);
        }
    }

    public void GetDamage(float damage)
    {
        damagingComponent.damage = damage;
    }
}
