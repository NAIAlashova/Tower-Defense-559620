using Godot;
using System;

[GlobalClass]
public partial class SlowingComponent : DamagingComponent
{
    protected override void Damage(Node3D enemy)
    {
        foreach (var child in enemy.GetChildren())
            {
                if (child is MoveComponent moveComponent)
                {
                    moveComponent.Slowdown();
                    break;
                }
                if (child is Node3D node)
                {
                    MeshInstance3D mesh = node.GetChild<MeshInstance3D>(0, false);
                    if (mesh.GetActiveMaterial(0) is BaseMaterial3D material)
                    {
                        BaseMaterial3D newMaterial =(BaseMaterial3D)material.Duplicate();
                        Color originalColor = newMaterial.AlbedoColor;
                        newMaterial.AlbedoColor = originalColor.Lerp(Colors.Blue, 0.4f);
                        mesh.MaterialOverride = newMaterial;
                    }
                }
            }
    }
}
