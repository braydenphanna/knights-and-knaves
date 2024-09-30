using Godot;
using System;

public partial class Enemy : CharacterBody3D
{
	private AnimationPlayer animPlayer;
	public void setMesh(string m){
		Node mesh = GD.Load<PackedScene>("res://assets/"+m+".blend").Instantiate();
		AddChild(mesh);
		MoveChild(mesh,0);
		animPlayer = GetNode<AnimationPlayer>(m+"/AnimationPlayer");
		animPlayer.Play("Idle");
	}
}
