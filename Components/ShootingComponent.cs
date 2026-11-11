using Godot;
using System;

[GlobalClass]
public partial class ShootingComponent : Node
{
    protected CharacterBody3D parentNode;
    [Export]
    protected float Speed {set; get;} = 2f;
    [Export]
    protected float ShootingCooldown {set; get; } = 5f;
    private Node3D Target;
    [Export]
    private VisibleComponent visibleComponent;
    [Export]
    private float CooldownShotting = 1f;
    [Export]
    protected PackedScene BulletScene = GD.Load<PackedScene>("res://Scenes/bullet.tscn");
    protected float CooldownProgress = 0f;
    protected int isDo = 1;

    // Сохраняет ссылку на стреляющий объект.
    public override void _Ready()
    {
      parentNode = GetParent<CharacterBody3D>();  
    }

    // Обновляет цель, выполняет выстрел и отсчитывает перезарядку.
    public override void _Process(double delta)
    {
        // Проверяем наличие цели и готовность к выстрелу.
        if (IsInstanceValid(Target))
        {
            if (CooldownProgress <= 0.0)
            {
                if (isDo == 1)
                {
                    CooldownProgress = ShootingCooldown;
                    ShootBullet();
                }
            }
        }
        else if (visibleComponent != null)
        {
            // Запрашиваем цель у компонента обнаружения.
            Target = visibleComponent.GetTarget();
        }
        // Уменьшаем время перезарядки. Самодельный таймер
        CooldownProgress -= (float) delta;
    }

    // Создаёт пулю, задаёт урон и направляет её к цели.
    protected virtual void ShootBullet()
    {
        Bullet bulletInstance = BulletScene.Instantiate<Bullet>();
        bulletInstance.Position = parentNode.Position + new Vector3(0f, 1f, 0f);
        // Задаём параметры пуле и создаём её
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
        if (IsInstanceValid(Target))
        {
            bulletInstance.LookAt(Target.Position);
        }
        GD.Print("New bullet");
    }
    
    // Переключает разрешение стрельбы.
    public void Pause()
    {
        isDo = isDo * -1;
    }

    // Возвращает сцену снаряда для определения типа башни.
    public PackedScene CheckOfType()
    {
        return BulletScene;
    }
}
