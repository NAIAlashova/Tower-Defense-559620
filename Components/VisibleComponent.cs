using Godot;
using System;

[GlobalClass]
public partial class VisibleComponent : Area3D
{
    private Node3D parentNode;
    private Node3D currentTarget;
    
    public override void _Ready()
    {
        parentNode = GetParent<CharacterBody3D>();
    }

    public override void _Process(double delta)
    {
        
    }

    public Node3D GetTarget()
    {
        foreach (var item in GetOverlappingBodies())
        {
            bool isAlly = parentNode.IsInGroup("allies");
            bool isEnemy = parentNode.IsInGroup("enemies");
            if ((isAlly && item.IsInGroup("enemies")) || (isEnemy && item.IsInGroup("allies")))
            {
                if (IsInstanceValid(currentTarget))
                {
                    if (item.Position.DistanceTo(parentNode.Position) < currentTarget.Position.DistanceTo(parentNode.Position))
                    {
                        currentTarget = item;
                    }
                }
                else{currentTarget = item;}
            }
        }
        return currentTarget;
    }
}
