using Godot;
using System;

public partial class Enemy : CharacterBody3D
{
	private Node3D mesh;
	private AnimationPlayer animPlayer;
	public Enemy(Node3D m)
	{
		this.mesh = m;
		AddChild(this.mesh);
		animPlayer = GetNode<AnimationPlayer>(mesh.Name+"/AnimationPlayer");
		animPlayer.Play("Idle");
	}
}
