using Godot;
using System;

public partial class NpcBehavior : CharacterBody3D
{
	public AnimationPlayer animPlayer;

	public override void _Ready()
	{
		animPlayer = GetNode<AnimationPlayer>("character3/AnimationPlayer");
		animPlayer.Play("Idle");
	}
}
