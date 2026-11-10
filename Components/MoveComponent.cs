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
    private int isDo {set; get;} = 1;
    private Timer timerSlow;
    private bool isSlowed {set; get;} = false;
    private bool isAntiSlowed = false;

    public override void _Ready()
    {
      parentNode = GetParent<CharacterBody3D>();  
    }
    public override void _PhysicsProcess(double delta)
    {
        if (isSlowed && timerSlow != null)
        {
            if (timerSlow.IsStopped() & isAntiSlowed)
            {
                Speed = Speed * 4;
                isSlowed = false;
                timerSlow.QueueFree();
                timerSlow = null;
                isAntiSlowed = false;
            }
        }
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
        if (isDo == 1)
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
    
    public void Pause()
    {
        isDo = isDo * -1;
    }

    public virtual void Slowdown()
    {
        if (!isSlowed)
        {
            isSlowed = true;
            Speed = Speed / 4;
            timerSlow = new Timer();
            timerSlow.WaitTime = 20;
            timerSlow.OneShot = true;
            AddChild(timerSlow);
            timerSlow.Start();
            isAntiSlowed = true;
        }
    }
}
