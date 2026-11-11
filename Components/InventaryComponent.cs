using Godot;
using System;

[GlobalClass]
public partial class InventaryComponent : Node
{
    [Export]
    private int MaxResources = 500;
    private int currentResources;
    [Export]
    protected Label currentCurrencyLabel;
    [Export]
    private PackedScene StandartBullet = GD.Load<PackedScene>("res://Scenes/bullet.tscn");
    [Export]
    private int InitialResources = 100;
    
    /// Цена постройки башен
    /// 0 - Базовая башня с DamagingComponent
    /// 1 - Башня с SlowingСomponent
    /// 2 - Башня с ExplosiveСomponent
    private int[] TowersPrice = [100, 120, 220];

    /// Приз за уничтожение башен
    /// 0 - Базовая башня с DamagingComponent
    /// 1 - Башня с SlowingComponent
    /// 2 - Башня с ExplosiveСomponent
    private int[] TowersAward = [110, 120, 200];

    public override void _Ready()
    {
        currentResources = InitialResources;
        UpdateCurrencyLabel();
    }

    // Определяет тип башни и начисляет награду за уничтожение.
    public virtual void AddResources(Node3D tower)
    {
        foreach (var component in tower.GetChildren())
        {
            // Определяем башни и начисляем за них награды.
            if (component is ShootingComponent shooter)
            {
                if (shooter.CheckOfType() == StandartBullet)
                {
                    currentResources = currentResources + TowersAward[0];
                }
                else
                {
                    currentResources = currentResources + TowersAward[1];
                }
                break;
            }
            if (component is ExplosiveComponent)
            {
                currentResources = currentResources + TowersAward[2];
                break;
            }
        }
        // Обновляем надпись ресурсов.
        UpdateCurrencyLabel();
    }

    // Списывает стоимость выбранной башни и обновляет интерфейс.
    public virtual void DeleteResources(int type_tower)
    {
        currentResources = currentResources - TowersPrice[type_tower];
        // Выводим оставшиеся ресурсы.
        UpdateCurrencyLabel();
    }

    // Проверяет наличие ресурсов для выбранной башни.
    public bool CanBuildTower(int type_tower)
    {
        return currentResources >= TowersPrice[type_tower];
    }

    // Выводит текущее количество ресурсов в интерфейс.
    private void UpdateCurrencyLabel()
    {
        currentCurrencyLabel.Text = $"Ресурсы: {currentResources}";
    }
}
