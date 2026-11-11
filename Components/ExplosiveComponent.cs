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
}
