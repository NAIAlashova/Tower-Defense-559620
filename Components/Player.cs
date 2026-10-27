using Godot;
using System;
using System.ComponentModel;

[GlobalClass]
public partial class Player : CharacterBody3D
{
    private Vector3 _velocity;
    private Node3D parentNode;
    private int isDo = 1;
    public override void _Ready()
    {
      parentNode = GetParent<Node3D>();  
    }

    public override void _PhysicsProcess(double delta)
    {
        Move((float)delta);
        MoveAndSlide();
    }

    private void Move(float delta)
    {
        if (isDo == 1)
        {
            if (Input.IsActionPressed("forward"))
            {
                _velocity = new Vector3 (10f, 0f, 0f);
                Position += _velocity * delta;
            }
            if (Input.IsActionPressed("right"))
            {
                _velocity = new Vector3 (0f, 0f, 10f);
                Position += _velocity * delta;
            }
            if (Input.IsActionPressed("back"))
            {
                _velocity = new Vector3 (-10f, 0f, 0f);
                Position += _velocity * delta;
            }
            if (Input.IsActionPressed("left"))
            {
                _velocity = new Vector3 (0f, 0f, -10f);
                Position += _velocity * delta;
            }
        }
        if (Input.IsActionJustPressed("pause"))
        {
            foreach (var sibling in parentNode.GetChildren())
            {
                isDo = isDo * -1;
                foreach (var component in sibling.GetChildren())
                {
                    if (component is MoveComponent moveComponent)
                    {
                        moveComponent.Pause();
                    }
                    else if (component is ShootingComponent shootingComponent)
                    {
                        shootingComponent.Pause();
                    }
                }
            }
        }
    }
}
