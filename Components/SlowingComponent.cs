using Godot;
using System;

[GlobalClass]
public partial class SlowingComponent : DamagingComponent
{
    protected override void Damage(Node3D enemy)
    {
        if (enemy is Player player)
        {
            player.Slowdown();
        }
        else
        {
            foreach (var child in enemy.GetChildren())
                {
                    if (child is MoveComponent moveComponent)
                    {
                        moveComponent.Slowdown();
                        
                        break;
                    }
                }
        }
        /// Чместо изменения цвета создавай звездочку над замедленнысм объектом.
        /// enemy.AddChild();
    }
}
