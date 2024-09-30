using Godot;
using System;

public partial class FreeCam : CharacterBody3D
{
	private float Speed = 15.0f;
	private float JumpVelocity = 4.5f;
	private RayCast3D ray;
	[Export] public float sensitivityHorizontal = 0.5f;
	[Export] public float sensitivityVertical = 0.5f;
	CharacterBody3D selected;
	Node3D circle;
	public override void _Ready()
	{
		ray = GetNode<RayCast3D>("RayCast3D");
		selected = new CharacterBody3D();
		circle = GD.Load<PackedScene>("res://assets/circle.blend").Instantiate() as Node3D;
		GetParent().CallDeferred(MethodName.AddChild,circle);
	}
	public void init(){
		this.Transform = GetParent().GetNode<Camera3D>("Player/SpringArm3D/Camera3D").GlobalTransform;
		(GetChild(0)as Camera3D).Current = true;
	}

	public override void _Input(InputEvent e)
	{
		if(e is InputEventMouseMotion){
			InputEventMouseMotion m = (InputEventMouseMotion) e;
			Rotation -= new Vector3(m.Relative.Y/300,m.Relative.X/300,0).Clamp((float)-Math.PI/2,(float)Math.PI/2);	
		}
	}
	public override void _PhysicsProcess(double delta)
	{
		
		if(ray.IsColliding() && ray.GetCollider() is CharacterBody3D){
			if (Input.IsActionJustPressed("leftClick"))
			{
				selected = ray.GetCollider() as CharacterBody3D;
				circle.Position = new Vector3(selected.Position.X, 0, selected.Position.Z);
			}
		}

		

		Vector3 velocity = Velocity;

		Vector2 inputDir = Input.GetVector("left", "right", "foward", "back");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Y = direction.Y * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Y = Mathf.MoveToward(Velocity.Y, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}
}
