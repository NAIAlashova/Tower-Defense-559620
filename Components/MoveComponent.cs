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
    [Export(PropertyHint.Range, "0.1,120,0.1,or_greater")]
    private double SlowDuration = 20.0;
    private bool isSlowed {set; get;} = false;
    private bool isAntiSlowed = false;
    [Export]
    private PackedScene Simbol;


    // Сохраняет ссылку на перемещаемую башню.
    public override void _Ready()
    {
      parentNode = GetParent<CharacterBody3D>();  
    }

    public override void _PhysicsProcess(double delta)
    {
        // После таймера восстанавливаем скорость и удаляем таймер.
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
        // Запрашиваем новую цель, если прежняя недействительна.
        if (!IsInstanceValid(Target))
        {
            Target = visibleComponent.GetTarget();
        }
        else
        {
            // Двигаемся к действующей цели.
            _Move((float)delta);
        }
    }
    // Перемещает башню с учётом паузы и радиуса остановки.
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
            // Если цели нет, движемся к центру
            else
            {
                Vector3 _velocity = new Vector3(0f, 0f, 0f) - Target.Position;
                _velocity.Y = 0f;
                parentNode.Position += _velocity.Normalized() * Speed * delta;
                parentNode.MoveAndSlide();
            }
        }
    }
    
    // Переключает разрешение движения для режима строительства.
    public void Pause()
    {
        isDo = isDo * -1;
    }

    // Замедляет башню в четыре раза на 20 секунд.
    public virtual void Slowdown()
    {
        if (!isSlowed)
        {
            isSlowed = true;
            Speed = Speed / 4;
            timerSlow = new Timer();
            timerSlow.WaitTime = SlowDuration;
            timerSlow.OneShot = true;
            AddChild(timerSlow);
            timerSlow.Start();
            isAntiSlowed = true;

            /// Создаёт символ, над замедленным объектом для подтверждения наложения эффекта.
            Area3D simbolInstace = Simbol.Instantiate<Area3D>();
            simbolInstace.Position = new Vector3(0f, 2.7f, 0f);
            parentNode.AddChild(simbolInstace);
        }
    }
}
