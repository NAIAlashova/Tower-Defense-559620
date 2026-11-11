using Godot;
using System;

[GlobalClass]
public partial class TrackerComponent : Node
{
    [Export]
    private InventaryComponent inventaryNode;

    // Обрабатывает гибель персонажа.
    public virtual void DieObject(Node3D body)
    {
        // При гибели игрока или ядра запускаем конец игры.
        if (body is Player | body is Core)
        {
              GetTree().CurrentScene.QueueFree();
        }
        else
        {
            // За уничтожение врага начисляем ресурсы.
            if (body.IsInGroup("enemies"))
            {
                inventaryNode.AddResources(body);
            }
        }
    }
}
