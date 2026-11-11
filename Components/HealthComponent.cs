using Godot;
using System;

[GlobalClass]
public partial class HealthComponent : Node
{
    private Node3D parentNode;
    private TrackerComponent trackerComponent;
    [Export]
    protected float MaxHealth {get; set; } = 100f;
    public float CurrentHealth { get; private set; }
    public event Action<float> HealthChanged;

    public override void _Ready()
    {
        parentNode = GetParent<CharacterBody3D>();
        CurrentHealth = MaxHealth;
        foreach (var child in parentNode.GetParent().GetChildren())
        {
            if (child is TrackerComponent traker)
            {
                trackerComponent = traker;
            }
        }
    }

    // Обрабатывает получаемый урон.
    public void TakeDamage(float damage)
    {
        CurrentHealth -= damage;
        HealthChanged?.Invoke(CurrentHealth);
        if (CurrentHealth < 1)
        {
            Die();
        }
    }

    // Обрабатывает смерть
    private void Die()
    {
        GD.Print($"{parentNode.Name} die");
        trackerComponent.DieObject(parentNode);
        parentNode.QueueFree();
    }
}
