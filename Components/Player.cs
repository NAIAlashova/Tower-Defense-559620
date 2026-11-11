using Godot;
using System;
using System.ComponentModel;

[GlobalClass]
public partial class Player : CharacterBody3D
{
    private Vector3 _velocity;
    private Node3D parentNode;
    private int isDo = 1;
    private Timer timerSlow;
    private bool isSlowed {set; get;} = false;
    [Export]
    protected PackedScene TestTowerScene;
    [Export]
    private Label ConstructionLabel;
    [Export]
    private Label HealthLabel;
    [Export]
    private Label CanConstructionLabel;
    [Export]
    private Timer CanConstructionTimer;
    private CharacterBody3D testTowerInstance;
    private int towerType = 0;
    private string[] typeTowerScene = ["res://Scenes/tower(t1).tscn", 
    "res://Scenes/tower(t2).tscn", 
    "res://Scenes/tower(t3).tscn"];
    [Export]
    private PackedScene Simbol;

    public override void _Ready()
    {
      parentNode = GetParent<Node3D>();
      // Сигнал на изменение здоровья и выводим значение текущего здоровья.
      var health = GetNode<HealthComponent>("HealthComponent");
      health.HealthChanged += UpdateHealthLabel;
      UpdateHealthLabel(health.CurrentHealth);
    }

    // Выводит здоровье игрока без отрицательных значений.
    private void UpdateHealthLabel(float health)
    {
        if (HealthLabel != null)
            HealthLabel.Text = $"Здоровье: {Mathf.Max(0f, health):0.#}";
    }

    // Обновляет уведомление, движение и направление взгляда.
    public override void _PhysicsProcess(double delta)
    {
        // Скрываем сообщение после окончания таймера.
        if (CanConstructionTimer.IsStopped())
        {
            CanConstructionLabel.Visible = false;
        }
        // Обрабатываем перемещение игрока.
        Move((float)delta);
        MoveAndSlide();
        RotateTowardsMouse();
    }

    // Поворот игрока в сторону мыши
    private void RotateTowardsMouse()
    {
        Camera3D camera = GetViewport().GetCamera3D();
        if (camera == null)
            return;

        // Берём позицию мыши на экране
        Vector2 mousePosition = GetViewport().GetMousePosition();
        // Получаем луч из камеры в направлении мыши (находит прямую, которой этот луч принаджежит)
        Vector3 rayOrigin = camera.ProjectRayOrigin(mousePosition);     
        // Направление луча.
        Vector3 rayDirection = camera.ProjectRayNormal(mousePosition);

        // Создаёт плоскость на уровне игрока для нахождения пересечения
        Plane aimPlane = new Plane(Vector3.Up, GlobalPosition.Y);
        Vector3? intersection = aimPlane.IntersectsRay(rayOrigin, rayDirection);
        if (!intersection.HasValue)
            return;

        // Поворачиваемся к найденной точке, избегая нулевого направления.
        Vector3 target = intersection.Value;
        target.Y = GlobalPosition.Y;
        if (GlobalPosition.DistanceSquaredTo(target) > 0.0001f)
            LookAt(target, Vector3.Up);
    }

    // Перемещает игрока по вводу с учётом замедления.
    private void Move(float delta)
    {
        if (isDo == 1)
        {
            if (Input.IsActionPressed("forward"))
            {
                _velocity = new Vector3 (10f, 0f, 0f);
                _velocity = SlowVelocity(_velocity);
                Position += _velocity * delta;
            }
            if (Input.IsActionPressed("right"))
            {
                _velocity = new Vector3 (0f, 0f, 10f);
                _velocity = SlowVelocity(_velocity);
                Position += _velocity * delta;
            }
            if (Input.IsActionPressed("back"))
            {
                _velocity = new Vector3 (-10f, 0f, 0f);
                _velocity = SlowVelocity(_velocity);
                Position += _velocity * delta;
            }
            if (Input.IsActionPressed("left"))
            {
                _velocity = new Vector3 (0f, 0f, -10f);
                _velocity = SlowVelocity(_velocity);
                Position += _velocity * delta;
            }
        }
    }

    // Обрабатывает действия с клавиатуры.
    public override void _Input(InputEvent @event)
    {
        if (@event.IsEcho())
            return;

        if (@event.IsActionPressed("pause"))
        {
            isDo *= -1;

            // Переключаем паузу движения и стрельбы объектов сцены.
            foreach (var sibling in parentNode.GetChildren())
            {
                foreach (var component in sibling.GetChildren())
                {
                    if (component is MoveComponent moveComponent)
                        moveComponent.Pause();
                    else if (component is ShootingComponent shootingComponent)
                        shootingComponent.Pause();
                    else if (component is SpawnerComponent spawnerComponent)
                        spawnerComponent.Pause();
                }
            }
            InstanceMenu();
        }

        // Обрабатываем выбор типа башни в режиме строительства.
        if (isDo == -1)
        {
            if (@event.IsActionPressed("tower_type1"))
                towerType = 0;
            else if (@event.IsActionPressed("tower_type2"))
                towerType = 1;
            else if (@event.IsActionPressed("tower_type3"))
                towerType = 2;
        }
    }

    // При первом вызове прозволяет выбрать башню, создавая макет.
    // При повторном строит выбранную башню при наличии нужного количества ресурсов.
    private void InstanceMenu()
    {
        // В режиме строительства показываем макет башни.
        if (isDo == -1)
        {
            /// Создаём меню
            testTowerInstance = TestTowerScene.Instantiate<CharacterBody3D>();
            testTowerInstance.Position = parentNode.Position + new Vector3(0f, 0f, -1f);
            AddChild(testTowerInstance);

            ConstructionLabel.Visible = true;
            /// Выбор типа башни происходит в методе _Input
        }
        else
        {
            // Проверяем ресурсы для выбранной башни.
            if (parentNode.GetNode<InventaryComponent>("InventaryComponent").CanBuildTower(towerType))
            {
                // Создаём башню на месте макета.
                PackedScene TowerScene = GD.Load<PackedScene>(typeTowerScene[towerType]);
                CharacterBody3D TowerInstance = TowerScene.Instantiate<CharacterBody3D>();
                TowerInstance.Position = testTowerInstance.GlobalPosition;
                AddSibling(TowerInstance);

                // Списываем стоимость постройки.
                parentNode.GetNode<InventaryComponent>("InventaryComponent").DeleteResources(towerType);
            }
            else
            {
                // Показываем временное сообщение о нехватке ресурсов.
                CanConstructionLabel.Visible = true;
                CanConstructionTimer.Start();
            }

            // Скрываем подсказку и удаляем макет.
            ConstructionLabel.Visible = false;
            testTowerInstance.QueueFree();
        }
    }

    // Включает замедление игрока на 20 секунд через таймер.
    public virtual void Slowdown()
    {
        if (!isSlowed)
        {
            isSlowed = true;
            timerSlow = new Timer();
            timerSlow.WaitTime = 20;
            timerSlow.OneShot = true;
            AddChild(timerSlow);
            timerSlow.Start();
            /// Создаёт символ, над замедленным объектом для подтверждения наложения эффекта.
            Area3D simbolInstace = Simbol.Instantiate<Area3D>();
            simbolInstace.Position = new Vector3(0f, 1.7f, 0f);
            AddChild(simbolInstace);
        }
    }

    // Возвращает скорость с учётом активного замедления.
    // Пока таймер работает, уменьшаем скорость втрое.
    private Vector3 SlowVelocity(Vector3 _velocity)
    {
        if (timerSlow != null)
            {                
                if (!timerSlow.IsStopped())
                {
                    _velocity = _velocity / 3;
                }
                else
                {
                    // После таймера разрешаем повторное замедление.
                    isSlowed = false;
                }
            }
        return _velocity;
    }
}
