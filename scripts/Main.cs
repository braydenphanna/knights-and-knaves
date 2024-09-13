using Godot;
using System;
using System.Numerics;

public partial class Main : Node3D
{
	Camera3D cam;
	Node3D player;
	CharacterBody3D playercb;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		cam = GetNode<Camera3D>("Camera3D");
		player = GetNode<Node3D>("Player");
		playercb = GetNode<CharacterBody3D>("Player/CharacterBody3D");
		
		GD.Print("Ready");
	}


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		Godot.Vector3 v3 = player.Position + playercb.Position;
		cam.Position = new Godot.Vector3(v3.X, cam.Position.Y, cam.Position.Z);
		cam.LookAt(v3);
	}
}
