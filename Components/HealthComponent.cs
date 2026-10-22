using Godot;
using System;

[GlobalClass]
public partial class HealthComponent : Node
{
    private Node3D parentNode;
    [Export]
    protected float MaxHealth {get; set; } = 100f;
    protected float CurrentHealth {get; set; }
    
    public override void _Ready()
    {
        parentNode = GetParent<CharacterBody3D>();
        CurrentHealth = MaxHealth;
    }

    public void TakeDamage(float damage)
    {
        CurrentHealth -= damage;
        if (CurrentHealth < 1)
        {
            Die();
        }
    }

    private void Die()
    {
        GD.Print($"{parentNode.Name} die");
        parentNode.QueueFree();
    }
}
