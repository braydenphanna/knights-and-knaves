using Godot;
using System;

public partial class Enemy : CharacterBody3D
{
	private Node3D mesh;
	public Enemy(Node3D m)
	{
		this.mesh = m;
		AddChild(this.mesh);
	}
}
