using Godot;
using System;

public partial class bullet : Node3D
{
	const float Speed = 40.0f;

    [Export]
    public MeshInstance3D BulletMash { get; set; }
    [Export]
	public RayCast3D BulletRayCast { get; set; }
	[Export]
	public GpuParticles3D Particles { get; set; }
    [Export]
    public Timer Timer { get; set; }


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        Timer.Timeout += OnTimerTimeout;
	}

    private void OnTimerTimeout()
    {
        this.QueueFree();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
        var fdelta = (float)delta;

        var vec = new Vector3(0, 0, -Speed) * fdelta;
        Position += Transform.Basis * vec;
		if (BulletRayCast.IsColliding())
		{
            var collider = (StaticBody3D)BulletRayCast.GetCollider();

            if (collider.IsInGroup("targets"))
            {
                var gui = (GUI)GetTree().CurrentScene.GetNode("Gui");
                gui.Points += 1;
            }

			BulletMash.Visible = false;
			Particles.Emitting = true;

            
		}
    }

	
}
