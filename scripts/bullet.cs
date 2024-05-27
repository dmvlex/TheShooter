using Godot;
using System;
using TheShooter.scripts;

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
            if (BulletRayCast.GetCollider() is BaseTarget collider)
            {
                var gui = (GUI)GetTree().CurrentScene.GetNode("Gui");
                
                if (collider.EnabledToShot)
                {
                    collider.DetectShoot();

                    if (collider is GreenTarget) gui.Points += 10;
                    if (collider is OrangeTarget) gui.Points += 50;
                    if (collider is RedTarget) gui.Points += 150;

                }
            }

			BulletMash.Visible = false;
			Particles.Emitting = true;

            
		}
    }

	
}
