using Godot;
using System;

[GlobalClass]
public partial class PlayersShootingComponent : ShootingComponent
{
    public override void _Process(double delta)
    {
        if (isDo == 1)
            {
                if (Input.IsActionJustPressed("shoot"))
                {
                    ShootBullet();
                }
            }
    }

    protected override void ShootBullet()
    {
        Bullet bulletInstance = BulletScene.Instantiate<Bullet>();
        bulletInstance.Position = parentNode.Position + new Vector3(0f, 1f, 0f);
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

        bulletInstance.Rotation = parentNode.Rotation;
        GD.Print("Bullet");
    }
}
