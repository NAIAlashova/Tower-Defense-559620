using Godot;
using System;

[GlobalClass]
public partial class TrackerComponent : Node
{
    [Export]
    private InventaryComponent inventaryNode;

    public virtual void DieObject(Node3D body)
    {
        if (body is Player)
        {
            /// Удаляем всю сцену
        }
        else
        {
            if (body.IsInGroup("enemies"))
            {
                inventaryNode.AddResources(body);
            }
        }
    }
}
