using Godot;
using System;

[GlobalClass]
public partial class TrackerComponent : Node
{
    [Export]
    private InventaryComponent inventaryNode;
    [Export]
    private Label EndLabel;

    // Обрабатывает гибель персонажа.
    public virtual void DieObject(Node3D body)
    {
        // При гибели игрока или ядра запускает конец игры.
        if (body is Player | body is Core)
        {
            if(body.IsInGroup("allies"))
            {
                EndLabel.Text = "You lose";
            }
            else
            {
                EndLabel.Text = "You win";
            }

            // Удаляет лишние узлы.
            var nodes = new System.Collections.Generic.Stack<Node>();
            nodes.Push(GetTree().CurrentScene);
            while (nodes.Count > 0)
            {
                Node node = nodes.Pop();
                foreach (Node child in node.GetChildren())
                {
                    nodes.Push(child);
                }
                if (node is CharacterBody3D || (node is Label & node != EndLabel))
                {
                    node.QueueFree();
                }
            }
            
            EndLabel.Visible = true;
        }
        // За уничтожение врага начисляем ресурсы.
        else
        {
            if (body.IsInGroup("enemies"))
            {
                inventaryNode.AddResources(body);
            }
        }
    }
}
