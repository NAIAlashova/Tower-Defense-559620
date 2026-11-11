using Godot;
using System;

[GlobalClass]
public partial class PlayersShootingComponent : ShootingComponent
{
    // Стреляет по нажатию действия shoot, если стрельба разрешена.
    // Запускает самодельный таймер для учёта кулдауна.
    public override void _Process(double delta)
    {
        if (isDo == 1 & CooldownProgress <= 0.0)
            {
                if (Input.IsActionJustPressed("shoot"))
                {
                    ShootBullet();
                    CooldownProgress = ShootingCooldown;
                }
            }
        CooldownProgress -= (float) delta;
    }

    protected override void ShootBullet()
    {
        Bullet bulletInstance = BulletScene.Instantiate<Bullet>();
        bulletInstance.Position = parentNode.Position + new Vector3(0f, 1f, 0f);
        // Задаёс параметры пуле и инстанцируём её.
        bool isAlly = parentNode.IsInGroup("allies");
        if (isAlly)
        {
            bulletInstance.damagingGroup = "enemies";
        }
        else
        {
            bulletInstance.damagingGroup = "allies";
        }
        bulletInstance.parentNode = parentNode;
        parentNode.AddSibling(bulletInstance);
        if (parentNode is Player)
        {
            bulletInstance.GetDamage(10f);
        }
        else
        {
            bulletInstance.GetDamage(20f);
        }
        // Копируем поворот стрелка для направления полёта.
        // Единственное отличие метода от родительского
        bulletInstance.Rotation = parentNode.Rotation;
        GD.Print("Bullet");
    }
}
