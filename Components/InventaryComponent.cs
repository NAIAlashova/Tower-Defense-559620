using Godot;
using System;

[GlobalClass]
public partial class InventaryComponent : Node
{
    [Export]
    private int MaxResources = 500;
    private int currentResources;
    [Export]
    private int InitialResources = 100;
    /// <summary>
    /// Цена постройки башен
    /// 0 - Базовая башня с DamagingComponent
    /// 1 - Башня с SlowingСomponent
    /// </summary>
    private int[] TowersPrice = [100, 120];
    /// <summary>
    /// 0 - Базовая башня с DamagingComponent
    /// 1 - Башня с SlowingComponent
    /// 2 - Башня с ExplosiveСomponent
    /// </summary>
    private int[] TowersAward = [100, 120, 200];

    public override void _Ready()
    {
        currentResources = InitialResources;
    }

    public virtual void AddResources(Node3D tower)
    {
        foreach (var component in tower.GetChildren())
        {
            if (component is ShootingComponent)
            {
                currentResources = currentResources + TowersAward[0];
            }
            if (component is SlowingComponent)
            {
                currentResources = currentResources + TowersAward[1];
                break;
            }
            if (component is ExplosiveComponent)
            {
                currentResources = currentResources + TowersAward[2];

            }
        }
    }

    public virtual void DeleteResources(int type_tower)
    {
        currentResources = currentResources - TowersPrice[type_tower];
    }
}
