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
        bulletInstance.Position = parentNode.Position;
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

        Camera3D camera = GetViewport().GetCamera3D();
        /// Формируем вектор от камеры до позиции мыши в 2D, так как экран двухмерный
        Vector2 mousePosition = GetViewport().GetMousePosition();
        /// Начальная точка вектора3D
        Vector3 rayOrigin = camera.ProjectRayOrigin(mousePosition);
        /// конечная точка вектора3D
        Vector3 rayEnd = rayOrigin + camera.ProjectRayNormal(mousePosition) * 1000f;
        // Делаем луч, который фиксирует пересечение с физическими объектами
        PhysicsRayQueryParameters3D query = PhysicsRayQueryParameters3D.Create(rayOrigin, rayEnd);
        var result = camera.GetWorld3D().DirectSpaceState.IntersectRay(query);
        if (result.Count > 0)
        {
            Vector3 targetPosition = (Vector3)result["position"];
            bulletInstance.LookAt(targetPosition, Vector3.Up);
        }
        GD.Print("Bullet");
    }
}
