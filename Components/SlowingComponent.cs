using Godot;
using System;

[GlobalClass]
public partial class SlowingComponent : DamagingComponent
{
    // Заменяет нанесение урона замедлением цели.
    protected override void Damage(Node3D enemy)
    {
        // Для игрока вызываем его собственное замедление.
        if (enemy is Player player)
        {
            player.Slowdown();
        }
        else
        {
            // У остальных целей ищем и замедляем компонент движения.
            foreach (var child in enemy.GetChildren())
                {
                    if (child is MoveComponent moveComponent)
                    {
                        moveComponent.Slowdown();
                        
                        break;
                    }
                }
        }
    }
}
