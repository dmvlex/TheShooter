using Godot;
using System;

public partial class FPC : CharacterBody3D
{
    #region redactor settings
    [Export]
    public PlayerMovementConfig playerConfig;
	private PlayerMovement movement;
    #endregion

    public override void _Ready()
    {
        base._Ready();
		playerConfig.RootNode = this;
		movement = new PlayerMovement(playerConfig);

        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        //rotate the head
        if (@event is InputEventMouseMotion eventMouseMotion)
        {
			movement.RotateHeadByMouse(eventMouseMotion.Relative);
        }
    }

    public override void _PhysicsProcess(double delta)
	{
		movement._PhysicsProcess(delta);
	}
}

