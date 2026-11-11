using Godot;
using System;

[GlobalClass]
public partial class ComponentForTemporaryBody : Node
{
    [Export]
    private Timer TimerOfLife;
    private Node3D parentNode;

    // Запускает таймер существования
    public override void _Ready()
    {
        parentNode = GetParent<Node3D>();
        TimerOfLife.Start();
    }

    // Удаляет родительский узел по завершении таймера
    private void _on_timer_timeout()
    {
        GD.Print($"{parentNode.Name} die for time");
        parentNode.QueueFree();
    }
}
