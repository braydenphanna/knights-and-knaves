using Godot;
using System;
using System.Linq;

public partial class CameraAngleController : Node
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
		Node col = GetNode<Node>("Col");
		for(int i = 0; i < col.GetChildCount(); i++){
			StaticBody3D current = col.GetChild<StaticBody3D>(i);
			Area3D area = new Area3D();
			CollisionShape3D collider = new CollisionShape3D();
			current.GetNode<CollisionShape3D>("CollisionShape3D").Disabled =true;
			collider.Shape = current.GetNode<CollisionShape3D>("CollisionShape3D").Shape;
			area.AddChild(collider);
			current.AddChild(area);
			area.Monitoring = true;
			area.BodyEntered += (Node3D body) => playerEntered(body, col, current);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}
	public void playerEntered(Node3D body, Node col, StaticBody3D current){
		if(body.Name.Equals("Player")){
			GD.Print(current.GetChild<Camera3D>(0).Name);
			col.GetParent().GetParent().GetNode<Camera3D>("Camera3D").Transform = current.GetChild<Camera3D>(0).GlobalTransform;
		}
	}
}
