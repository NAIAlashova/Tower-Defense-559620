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

    // Сохраняет родительский узел.
    public override void _Ready()
    {
        bulletParentNode = GetParent<Area3D>();
    }

    // Обрабатывает столкновение с целью или препятствием.
    public virtual void _on_body_entered(Node3D body)
    {
        // Проверяем принадлежность тела к группе целей.
        if (body.IsInGroup(damagingGroup))
        {
            Hit(body);
        }
        // Удаляем пулю при контакте с препятствием, кроме стрелка и самой пули.
        else if (body != shootedNode && body != bulletParentNode)
        {
            bulletParentNode.QueueFree();
        }
    }

    // Применяет действие попадания и удаляет родительский объект.
    protected virtual void Hit(Node3D body)
    {
        GD.Print($"{bulletParentNode.Name} in enemie");
        Damage(body);
        bulletParentNode.QueueFree();
    }

    // Находит здоровье цели и наносит урон.
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
