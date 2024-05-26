using Godot;
using System;
using System.Runtime.CompilerServices;

public class PlayerMovementMachine 
{
	private readonly PlayerMovementConfig config;
    private float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

    private float fdelta;
    private float t_bob = 0.0f;
    private float default_head_y = 1.04f;
    private float currentSpeed = 0f;
    private float crouching_depth = -0.5f;


    private Node3D head;
    private Camera3D eyeCamera;
    private CollisionShape3D sneakCollision;
    private CollisionShape3D standCollision;
    private RayCast3D topRayCast;

    Vector3 direction = Vector3.Zero;

    public PlayerMovementMachine(PlayerMovementConfig config)
    {
		this.config = config;
		GetPlayerNodes();
    }

	private void GetPlayerNodes()
	{
        head = config.RootNode.GetNode<Node3D>("Head");
        eyeCamera = head.GetNode<Camera3D>("EyeCamera");
        standCollision = config.RootNode
            .GetNode<CollisionShape3D>("StandingCollisionShape");
        sneakCollision = config.RootNode
            .GetNode<CollisionShape3D>("SneakCollisionShape");
        topRayCast = config.RootNode.GetNode<RayCast3D>("TopRayCast");
	}

    public void RotateHeadByMouse(Vector2 mouseRelative)
    {
        var rotDeg = config.RootNode.RotationDegrees;

        rotDeg.Y -= mouseRelative.X * config.MouseSensitivity;
        config.RootNode.RotationDegrees = rotDeg;
        rotDeg = head.RotationDegrees;
        rotDeg.X -= mouseRelative.Y * config.MouseSensitivity;
        rotDeg.X = Mathf.Clamp(rotDeg.X, -89, 89);

        head.RotationDegrees = rotDeg;
    }

    public void _PhysicsProcess(double delta)
    {
        var root = config.RootNode;
        var fdelta = (float)delta;

        Vector3 velocity = root.Velocity;
        // Add the gravity.
        if (!root.IsOnFloor())
            velocity.Y -= gravity * fdelta;

        if (Input.IsActionPressed("Sneak"))
        {
            var a = default_head_y + crouching_depth;
            currentSpeed = config.SneakSpeed;
            eyeCamera.Fov = Mathf.Lerp(eyeCamera.Fov, config.BaseFOV - 15f, fdelta * 2.0f);
            head.Position = new Vector3(head.Position.X, Mathf.Lerp(head.Position.Y, a, fdelta * config.LerpSpeed), head.Position.Z);
            standCollision.Disabled = true;
            sneakCollision.Disabled = false;
        }
        else if (!topRayCast.IsColliding())
        {
            standCollision.Disabled = false;
            sneakCollision.Disabled = true;
            head.Position = new Vector3(head.Position.X,
                Mathf.Lerp(head.Position.Y, default_head_y, fdelta * config.LerpSpeed), head.Position.Z);

            if (Input.IsActionPressed("Run"))
            {
                currentSpeed = config.SprintSpeed;
                eyeCamera.Fov = Mathf.Lerp(eyeCamera.Fov, config.BaseFOV + 15f, fdelta * 2.0f);
            }
            else
            {
                eyeCamera.Fov = Mathf.Lerp(eyeCamera.Fov, config.BaseFOV, fdelta * 2.0f); ;
                currentSpeed = config.Speed;
            }

            // Handle Jump.
            if (Input.IsActionJustPressed("Jump") && root.IsOnFloor())
            {
                velocity.Y = config.JumpVelocity;
            }
        }

        // Get the input direction and handle the movement/deceleration.
        // As good practice, you should replace UI actions with custom gameplay actions.
        Vector2 inputDir = Input.GetVector("Move_Left", "Move_Right", "Move_Forward", "Move_Back");


        direction = direction.Lerp((root.Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized()
            , fdelta * config.LerpSpeed);

        if (direction != Vector3.Zero)
        {
            if (!root.IsOnFloor())
            {
                velocity.X = Mathf.Lerp(velocity.X, direction.X * (currentSpeed / 2f), fdelta * 2.0f);
                velocity.Z = Mathf.Lerp(velocity.Z, direction.Z * (currentSpeed / 2f), fdelta * 2.0f);
            }
            else
            {
                velocity.X = direction.X * currentSpeed;
                velocity.Z = direction.Z * currentSpeed;
            }
        }
        else
        {
            velocity.Z = 0.0f; //Mathf.MoveToward(Velocity.Z, 0, Speed);
            velocity.X = 0.0f; //Mathf.MoveToward(Velocity.X, 0, Speed);
        }

        root.Velocity = velocity;


        var floatIsOnFloor = root.IsOnFloor() ? 1f : 0f;

        t_bob += fdelta * velocity.Length() * floatIsOnFloor;
        eyeCamera.Position = HeadBob(t_bob);

        root.MoveAndSlide();
    }
    private Vector3 HeadBob(float time)
    {
        var pos = Vector3.Zero;

        pos.Y = MathF.Sin(time * config.ShakingFrequency) * config.ShakingAmplitude;
        pos.X = MathF.Cos(config.ShakingFrequency / 2) * config.ShakingAmplitude;
        return pos;
    }
}
