using Godot;
using System;

public partial class GUI : Control
{
	[Export]
	public int Points { get; set; } = 0;
	[Export]
	public Label PointsOutput { get; set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		PointsOutput.Text = Points.ToString();
	}
}
