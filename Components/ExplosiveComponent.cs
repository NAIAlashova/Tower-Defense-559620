using Godot;
using System;

[GlobalClass]
public partial class ExplosiveComponent : DamagingComponent
{
    private PackedScene DebrisScene = GD.Load<PackedScene>("res://Scenes/DebrisScene.tscn");
    
    // Сохраняет взрывную башню и задаёт урон.
    public override void _Ready()
    {
        bulletParentNode = GetParent<CharacterBody3D>();
        damage = 70f;
        GD.Print("Bomb is in the Scene");
    }

    // Запускает попадание при контакте с противником.
    public override void _on_body_entered(Node3D body)
    {
        if (body.IsInGroup(damagingGroup))
        {
            Hit(body);
        }
    }

    protected override void Hit(Node3D body)
    {
        GD.Print($"{bulletParentNode.Name} in enemie");
        Damage(body);

        GD.Print("Bomb");
        // Эффект взрыва.
        var random = new RandomNumberGenerator();
        random.Randomize();
        for (int i = 0; i < 20; i++)
        {
            // Создаём обломок в точке взрыва.
            var debris = DebrisScene.Instantiate<RigidBody3D>();
            GetTree().CurrentScene.AddChild(debris);
            debris.GlobalPosition = GlobalPosition;
            // Выбираем случайное направление разлёта.
            Vector3 direction = new Vector3(
                random.RandfRange(-1f, 1f),
                random.RandfRange(0.2f, 1f),
                random.RandfRange(-1f, 1f)
            ).Normalized();

            // Придаём обломку движение и вращение.
            debris.ApplyCentralImpulse(direction * random.RandfRange(3f, 7f));
            debris.ApplyTorqueImpulse(new Vector3(
                random.RandfRange(-2f, 2f),
                random.RandfRange(-2f, 2f),
                random.RandfRange(-2f, 2f)));
        }
        bulletParentNode.QueueFree();
    }

    protected override void Damage(Node3D enemy)
    {
        // Увеличиваем радиус воздействия взрыва
        CollisionShape3D damageShape = null;
        foreach (var child in GetChildren())
        {
            if (child is CollisionShape3D collision && collision.Shape is SphereShape3D)
            {
                damageShape = collision;
                break;
            }
        }
        if (damageShape == null)
        {
            GD.PushError("ExplosiveComponent: не найдена дочерняя сферическая форма коллизии.");
            return;
        }
        var sphere = (SphereShape3D)damageShape.Shape.Duplicate();
        sphere.Radius = 2f;
        damageShape.SetDeferred(CollisionShape3D.PropertyName.Shape, sphere);

        foreach (var target in GetOverlappingBodies())
        {
            foreach (var child in target.GetChildren())
                {
                    if (child is HealthComponent healthComponent)
                    {
                        healthComponent.TakeDamage(damage);
                        break;
                    }
                }
        }
    }
}
