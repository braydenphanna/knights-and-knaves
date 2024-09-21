using Godot;
using Godot.Collections;
using Godot.NativeInterop;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;

public partial class Npc : CharacterBody3D
{
	private string name;
	private int pointer = 1;
	private AnimationPlayer animPlayer;
	private Json json;
	private RichTextLabel text;
	private string npcEvent = "";
	private string world = "home";
	
	public void init(string n)
	{
		animPlayer = GetNode<AnimationPlayer>("character3/AnimationPlayer");
		animPlayer.Play("Idle");
		name = n;
		SetMeta("name", name);
		text = GetParent().GetParent().GetNode<RichTextLabel>("UI/TextBox/Text");
		Godot.FileAccess file = Godot.FileAccess.Open("res://dialogue/area1.json", Godot.FileAccess.ModeFlags.Read);
		json = new Json();
		Error dialogue = json.Parse(file.GetAsText(),true);
	}
	public void dialogueCommandReceiver(string command)
	{
		if(command == "begin")
		{
			text.Text= json.Data.AsGodotDictionary()[name].AsGodotDictionary()[pointer.ToString()].AsGodotDictionary()["text"].ToString();
			pointer++;
		}
		else if(command == "next")
		{
			try{
				text.Text= json.Data.AsGodotDictionary()[name].AsGodotDictionary()[pointer.ToString()].AsGodotDictionary()["text"].ToString();	
				pointer++;
			}
			catch(KeyNotFoundException){
				try{
					npcEvent= json.Data.AsGodotDictionary()[name].AsGodotDictionary()[pointer.ToString()].AsGodotDictionary()["event"].ToString();
					/* All commands
					   tele | teleports player to location  | Example: tele/-9,1,40
					   wrld | changes the world      		| Example: wrld/arena
					   cmbt | engages combat                | Example: cmbt
					*/
					if(npcEvent.Substring(0,4) == "tele"){
						string[] posRaw = npcEvent.Substring(5).Split(",");
						Vector3 pos = new Vector3(posRaw[0].ToFloat(),posRaw[1].ToFloat(),posRaw[2].ToFloat());
						pointer = 1;
						
						GetParent().GetParent().GetNode<Player>("Player").teleport(pos);
						GetParent().GetParent().GetNode<Player>("Player").endDialogue();
					}
					else if(npcEvent.Substring(0,4) == "wrld"){
						Node main = GetParent().GetParent();

						main.GetChild(0).QueueFree();

						world = npcEvent.Substring(5);
						Node newWorld = GD.Load<PackedScene>("res://levels/"+world+".blend").Instantiate();
						main.AddChild(newWorld);
						main.MoveChild(newWorld, 0);

						pointer = 1;
						
						GetParent().GetParent().GetNode<Player>("Player").endDialogue();
						(GetParent() as NpcSignalDirector).spawnNpcs(world);
					}
					else if(npcEvent.Substring(0,4) == "cmbt"){
						pointer = 1;
						
						GetParent().GetParent().GetNode<Player>("Player").endDialogue();
						GetParent().GetParent().GetNode<CombatController>("CombatController").initiateCombat(new Enemy[]{new Enemy((Node3D)GD.Load<PackedScene>("res://assets/character3.blend").Instantiate())});
					}
				}
				catch(KeyNotFoundException){
					pointer = 1;
					GetParent().GetParent().GetNode<Player>("Player").endDialogue();
				}
			}
		}
	}
	public string getWorld()
	{
		return world;
	}
	
}
