using Godot;
using System;

public partial class Main : Node3D
{
	private string world = "home";
	public override void _Ready()
	{
		GetNode<NpcSignalDirector>("NpcSignalDirector").spawnNpcs(GetChild(0).Name);
	}
	
	public void Tele(string coords){
		string[] posRaw = coords.Split(",");
		Vector3 pos = new Vector3(posRaw[0].ToFloat(),posRaw[1].ToFloat(),posRaw[2].ToFloat());
		
		GetNode<Player>("Player").teleport(pos);
		GetNode<Player>("Player").endDialogue();
	}
	public void Wrld(string worldName){
		GetChild(0).QueueFree();

		world = worldName;
		Node newWorld = GD.Load<PackedScene>("res://levels/"+world+".blend").Instantiate();
		AddChild(newWorld);
		MoveChild(newWorld, 0);
		
		GetNode<Player>("Player").teleport(new Vector3(0,1,0));
		GetNode<Player>("Player").endDialogue();
		GetNode<NpcSignalDirector>("NpcSignalDirector").spawnNpcs(world);
	}
	public void Cmbt(){
		Wrld("arena");
		GetNode<CombatController>("CombatController").init();
		
		GetNode<Player>("Player").endDialogue();
		GetNode<CombatController>("CombatController").enterCombat(new Enemy[]{new Enemy(),
																			new Enemy(),
																			new Enemy()});
																		
	}
}
