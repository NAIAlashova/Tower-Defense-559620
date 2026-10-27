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
            // LookAt(GetGlobalMousePosition(), Vector3.Up);
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
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion mouseMotionEvent)
        {
            RotateY(Mathf.DegToRad(-mouseMotionEvent.Relative.X * 0.1f));
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
                InstanceMenu();
            }
        }
    }

    private void InstanceMenu()
    {
        if (isDo == -1)
        {
            /// Создаём меню
        }
        else
        {
            /// Удаляем меню
        }
    }
}
