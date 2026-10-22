using Godot;
using System;

[GlobalClass]
public partial class MoveComponent : Node
{
    protected CharacterBody3D parentNode;
    [Export]
    protected float Speed {set; get;} = 2f;
    [Export]
    protected float StopRadius {set; get; } = 5f;
    private Node3D Target;
    [Export]
    private VisibleComponent visibleComponent;

    public override void _Ready()
    {
      parentNode = GetParent<CharacterBody3D>();  
    }
    public override void _PhysicsProcess(double delta)
    {
        if (!IsInstanceValid(Target))
        {
            Target = visibleComponent.GetTarget();
        }
        else
        {
            _Move((float)delta);
        }
    }
    private void _Move(float delta)
    {
        if (IsInstanceValid(Target)) 
        {
            Vector3 _velocity = Target.Position - parentNode.Position;
            _velocity.Y = 0f;
            if (parentNode.Position.DistanceTo(Target.Position) > StopRadius)
            {
                parentNode.Position += _velocity.Normalized() * Speed * delta;
                parentNode.MoveAndSlide();
            }
        }
        else
        {
            Vector3 _velocity = new Vector3(0f, 0f, 0f) - Target.Position;
            _velocity.Y = 0f;
            parentNode.Position += _velocity.Normalized() * Speed * delta;
            parentNode.MoveAndSlide();
        }
    }
}
