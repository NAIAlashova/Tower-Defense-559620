using Godot;
using System;

[GlobalClass]
public partial class VisibleComponent : Area3D
{
    private Node3D parentNode;
    private Node3D currentTarget;
    
    // Сохраняет башню, для которой ищет цели.
    public override void _Ready()
    {
        parentNode = GetParent<CharacterBody3D>();
    }

    public override void _Process(double delta)
    {
        
    }

    // Выбирает ближайшего противника среди тел в области.
    public Node3D GetTarget()
    {
        // Перебираем тела внутри области обнаружения.
        foreach (var item in GetOverlappingBodies())
        {
            // Определяем команду владельца и отбираем противников.
            bool isAlly = parentNode.IsInGroup("allies");
            bool isEnemy = parentNode.IsInGroup("enemies");
            if ((isAlly && item.IsInGroup("enemies")) || (isEnemy && item.IsInGroup("allies")))
            {
                // Сравниваем расстояния с последующими подходящими телами и сохраняем ближайшую.
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
