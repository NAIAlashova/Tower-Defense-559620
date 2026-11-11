using Godot;
using System;

[GlobalClass]
public partial class DamagingComponent : Area3D
{
    [Export]
    public string damagingGroup;
    [Export]
    public float damage{set; get;}
    public Node3D shootedNode;
    protected Node3D bulletParentNode;

    public override void _Ready()
    {
        bulletParentNode = GetParent<Area3D>();
    }

    public void _on_body_entered(Node3D body)
    {
        if (body.IsInGroup(damagingGroup))
        {
            Hit(body);
        }
        else if (body != shootedNode && body != bulletParentNode)
        {
            bulletParentNode.QueueFree();
        }
    }

    private void Hit(Node3D body)
    {
        GD.Print("in enemie");
        Damage(body);
        bulletParentNode.QueueFree();
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
    }
}
