using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

public class PlayerMovement 
{
	private readonly PlayerMovementConfig config;
    private float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();


    private bool shootBlock = false;
    private float shakingTime = 0.0f;
    private float default_head_y = 1.04f;
    private float currentSpeed = 0f;
    private float crouching_depth = -0.5f;


    private Node3D head;
    private IEnumerable<Node> gunsCollection;
    private PackedScene bullet;
    private Camera3D eyeCamera;
    private CollisionShape3D sneakCollision;
    private CollisionShape3D standCollision;
    private RayCast3D topRayCast;

    Vector3 direction = Vector3.Zero;

    public PlayerMovement(PlayerMovementConfig config)
    {
		this.config = config;
		GetPlayerNodes();
    }

	private void GetPlayerNodes()
	{
        bullet = GD.Load<PackedScene>("res://Scenes/bullet.tscn");
        head = config.RootNode.GetNode<Node3D>("Head");
        eyeCamera = head.GetNode<Camera3D>("EyeCamera");
        gunsCollection = eyeCamera.GetNode<Node3D>("Guns").GetChildren();
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
        var gun = (Node3D)gunsCollection.ToArray()[0];

        Vector3 velocity = root.Velocity;
        // Add the gravity.
        if (!root.IsOnFloor())
            velocity.Y -= gravity * fdelta;

        var isSneak = Sneak(fdelta);

        if (!topRayCast.IsColliding() && !isSneak)
        {
            StandUp(fdelta);
            Run(fdelta, gun);
            Jump(root, ref velocity);
        }

        Walk(root, ref velocity, fdelta);
        ShakeCamera(fdelta, ref velocity, root);

        Shoot(gun, root);
        Reload(gun);

        root.MoveAndSlide();
    }

    private void Reload(Node3D Gun)
    {
        var animation = Gun.GetNode<AnimationPlayer>("Animation");
        if (Input.IsActionJustPressed("Reload"))
        {
            shootBlock = true;
            animation.Play("reload");
            shootBlock = false;
        }
    }
    private void Shoot(Node3D Gun, CharacterBody3D root)
    {
        if (shootBlock) return;

        var animation = Gun.GetNode<AnimationPlayer>("Animation");
        var barrel = Gun.GetNode<RayCast3D>("BarrelRayCast");
        if (Input.IsActionPressed("Shoot") && !animation.IsPlaying())
        {
            animation.Play("shoot");
            var curBullet = bullet.Instantiate<Node3D>();
            curBullet.Position = barrel.GlobalPosition;
            Transform3D newBulletTransform = curBullet.Transform;
            newBulletTransform.Basis = barrel.GlobalTransform.Basis;
            curBullet.Transform = newBulletTransform;

            root.GetParent().AddChild(curBullet);
        }
    }
    private bool Sneak(float fdelta)
    {
        if (Input.IsActionPressed("Sneak"))
        {
            var a = default_head_y + crouching_depth;
            currentSpeed = config.SneakSpeed;
            eyeCamera.Fov = Mathf.Lerp(eyeCamera.Fov, config.MinFOV, fdelta * 2.0f);
            head.Position = new Vector3(head.Position.X, Mathf.Lerp(head.Position.Y, a, fdelta * config.LerpSpeed), head.Position.Z);
            standCollision.Disabled = true;
            sneakCollision.Disabled = false;
            return true;
        }
        return false;
    }
    private void StandUp(float fdelta)
    {
        standCollision.Disabled = false;
        sneakCollision.Disabled = true;
        head.Position = new Vector3(head.Position.X,
            Mathf.Lerp(head.Position.Y, default_head_y, fdelta * config.LerpSpeed), head.Position.Z);
    }
    private void Jump(CharacterBody3D root, ref Vector3 velocity)
    {
        // Handle Jump.
        if (Input.IsActionJustPressed("Jump") && root.IsOnFloor())
        {
            velocity.Y = config.JumpVelocity;
        }
    }
    private void Run(float fdelta, Node3D Gun)
    {
        var animation = Gun.GetNode<AnimationPlayer>("Animation");
        if (Input.IsActionPressed("Run"))
        {
            currentSpeed = config.SprintSpeed;
            eyeCamera.Fov = Mathf.Lerp(eyeCamera.Fov, config.MaxFOV, fdelta * 2.0f);
        }
        else
        {
            eyeCamera.Fov = Mathf.Lerp(eyeCamera.Fov, config.BaseFOV, fdelta * 2.0f); ;
            currentSpeed = config.Speed;
        }
    }
    private void Stop(ref Vector3 velocity)
    {
        velocity.Z = 0.0f; //Mathf.MoveToward(Velocity.Z, 0, Speed);
        velocity.X = 0.0f; //Mathf.MoveToward(Velocity.X, 0, Speed);
    }
    private void Walk(CharacterBody3D root, ref Vector3 velocity, float fdelta)
    {
        // Get the input direction and handle the movement/deceleration.
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
            Stop(ref velocity);
        }

        root.Velocity = velocity;
    }
    private void ShakeCamera(float fdelta, ref Vector3 velocity, CharacterBody3D root)
    {
        var floatIsOnFloor = root.IsOnFloor() ? 1f : 0f;
        shakingTime += fdelta * velocity.Length() * floatIsOnFloor;
        eyeCamera.Position = HeadBob(shakingTime);
    }

    private Vector3 HeadBob(float time)
    {
        var pos = Vector3.Zero;

        pos.Y = MathF.Sin(time * config.ShakingFrequency) * config.ShakingAmplitude;
        pos.X = MathF.Cos(config.ShakingFrequency / 2) * config.ShakingAmplitude;
        return pos;
    }
}
