using Godot;
using System;

public partial class PlayerInput : CharacterBody3D
{
	public float Speed = 7.0f;
	public float JumpVelocity = 4.5f;
	[Export] public float sensitivityHorizontal = 0.5f;
	[Export] public float sensitivityVertical = 0.5f;
	public SpringArm3D springArm;
	public override void _Ready()
	{
		springArm = GetNode<SpringArm3D>("SpringArm3D");
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}
	public override void _Input(InputEvent e)
	{
		base._Input(@e);
		if(e is InputEventMouseMotion){
			InputEventMouseMotion m = (InputEventMouseMotion) e;
			RotateY(Mathf.DegToRad(-m.Relative.X*sensitivityHorizontal));
			float rotVert = Mathf.DegToRad(-m.Relative.Y*sensitivityVertical);
			springArm.RotateX(rotVert);
			springArm.Rotation = springArm.Rotation.Clamp((float)Mathf.DegToRad(-90),(float)Mathf.DegToRad(22.5));
		}
	}
	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		// Handle Jump.
		if (Input.IsActionJustPressed("jump") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
		}

		if (Input.IsActionJustPressed("interact") && GetMeta("canInteract").AsBool())
		{
			GD.Print("The player interacted with something");
		}

		if (Input.IsActionPressed("sprint"))
		{
			Speed = 11.0f;
		}
		else{
			Speed = 7.0f;
		}

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector("left", "right", "foward", "back");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}
}
