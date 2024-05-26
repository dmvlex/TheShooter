using Godot;
using System;

public partial class bullet : Node3D
{
	const float Speed = 40.0f;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        var fdelta = (float)delta;

        var vec = new Vector3(0, 0, Speed) * fdelta;
        Position += Transform.Basis * vec;
    }
}
