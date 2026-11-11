using Godot;
using System;

[GlobalClass]
public partial class SpawnerComponent : Node
{
    private string[] EnemyTower = [
        "res://Scenes/enemie_tower(t1).tscn",
        "res://Scenes/enemy_tower(t2).tscn",
        "res://Scenes/enemy_tower(t3).tscn"
    ];
    private Timer spawnTimer;
    [Export]
    private int SpawnTime = 4;
    private PackedScene TowerScene;
    private Node3D parentNode;
    private int isDo = 1;
    
    // Создаёт таймер для спавна
    public override void _Ready()
    {
        parentNode = GetParent<Core>();
        spawnTimer = new Timer();
        spawnTimer.WaitTime = SpawnTime;
        spawnTimer.OneShot = true;
        AddChild(spawnTimer);
    }

    public override void _Process(double delta)
    {
        // Если таймер остановлен и запущен игровой режим(а не строительство),
        // то создаёт башнб врага одного из 3 представленных вариантов.
        if (spawnTimer.IsStopped() & isDo == 1)
        {
            var random = new RandomNumberGenerator();
            random.Randomize();
            TowerScene = GD.Load<PackedScene>(EnemyTower[random.RandiRange(0, 2)]);
            CharacterBody3D towerInstanse = TowerScene.Instantiate<CharacterBody3D>();
            towerInstanse.Position = parentNode.GlobalPosition + new Vector3 (-1f, 0f, 0f);
            parentNode.AddSibling(towerInstanse);

            spawnTimer.Start();
        }
    }

    // Вызывается из Player в случае запуска/окончания режима строительства.
    public void Pause()
    {
        isDo = isDo * -1;
    }
}
