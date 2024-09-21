using Godot;
using System;

public partial class Main : Node3D
{
	public override void _Ready()
	{
		GetNode<NpcSignalDirector>("NpcSignalDirector").spawnNpcs(GetChild(0).Name);
	}
}
