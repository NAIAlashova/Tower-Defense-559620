using Godot;
using System;

[GlobalClass]
public partial class DamagingComponent : Node
{
    public string damagingGroup;
    [Export]
    public float damage{set; get;}
    public Node3D shootedNode;
    private Node3D bulletParentNode;

    public override void _Ready()
    {
        bulletParentNode = GetParent<Area3D>();
    }

    public void Hit(Node3D body)
    {
        if (body.IsInGroup(damagingGroup))
        {
            GD.Print("in enemie");
            Damage(body);
            bulletParentNode.QueueFree();
        }
        else if (body != shootedNode && body != bulletParentNode)
        {
            bulletParentNode.QueueFree();
        }
        GD.Print("Damage");
    }

    protected virtual void Damage(Node3D enemy)
    {
        foreach (var child in enemy.GetChildren())
            {
                if (child is HealthComponent healthComponent)
                {
                    healthComponent.TakeDamage(damage);
                    break;
                }
            }
        // Наложение визуальных эффектов на врага
    }
}
