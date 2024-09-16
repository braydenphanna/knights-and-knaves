using Godot;
using System;
using System.Threading;

public partial class PlayerInput : CharacterBody3D
{
	public float Speed = 5.0f;
	public float JumpVelocity = 4.5f;
	[Export] public float sensitivityHorizontal = 0.5f;
	[Export] public float sensitivityVertical = 0.5f;

	public bool wasAFK;
	[Export] public double AFKTimer = 60;

	public SpringArm3D springArm;
	public AnimationPlayer animPlayer;
	public Godot.Timer timer;

	public override void _Ready()
	{
		springArm = GetNode<SpringArm3D>("SpringArm3D");
		Input.MouseMode = Input.MouseModeEnum.Captured;
		animPlayer = GetNode<AnimationPlayer>("character3/AnimationPlayer");

		timer = new Godot.Timer();
		AddChild(timer);
		timer.Timeout += onTimeout;

		timer.WaitTime = AFKTimer;
		timer.OneShot = true;
		timer.Start();
	}
	public override void _Input(InputEvent e)
	{
		base._Input(@e);
		if(e is InputEventMouseMotion){
			if(wasAFK){
				springArm.Rotation = new Vector3(Mathf.DegToRad(-15),0,0);
				wasAFK=false;
			}
			timer.WaitTime = AFKTimer;
			timer.Start();
			
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

		if (Input.IsActionJustPressed("jump") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
		}

		if (Input.IsActionJustPressed("escape"))
		{
			Input.MouseMode = Input.MouseModeEnum.Visible;
		}

		if (Input.IsActionJustPressed("interact") && GetMeta("canInteract").AsBool())
		{
			GD.Print("The player interacted with something");
		}

		if (Input.IsActionPressed("sprint"))
		{
			Speed = 10.0f;
		}
		else{
			Speed = 5.0f;
		}

		Vector2 inputDir = Input.GetVector("left", "right", "foward", "back");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
			animPlayer.Play("Walk");
			timer.WaitTime =10;
			timer.Start();
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
			
			animPlayer.Play("Idle");
			if(timer.IsStopped())
			{
				springArm.Rotation = new Godot.Vector3(Mathf.DegToRad(-25),springArm.Rotation.Y,springArm.Rotation.Z);
				springArm.RotateY(Mathf.DegToRad(-0.25f));
			}
		}

		Velocity = velocity;
		MoveAndSlide();
	}
	public void onTimeout()
	{
		wasAFK=true;
	}
}
