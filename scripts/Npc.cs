using Godot;
using Godot.Collections;
using Godot.NativeInterop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

public partial class Npc : CharacterBody3D
{
	[Export] private string name;
	private int pointer = 1;
	private AnimationPlayer animPlayer;
	private Json json;
	private RichTextLabel text;
	private string npcEvent = "";
	
	[Signal] public delegate void dialogueEndEventHandler(string command);
	[Signal] public delegate void teleportEventHandler(Vector3 pos);

	public override void _Ready()
	{
		animPlayer = GetNode<AnimationPlayer>("character3/AnimationPlayer");
		animPlayer.Play("Idle");
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
					
					if(npcEvent.Substring(0,4) == "tele"){
						string[] posRaw = npcEvent.Substring(5).Split(",");
						Vector3 pos = new Vector3(posRaw[0].ToFloat(),posRaw[1].ToFloat(),posRaw[2].ToFloat());
						pointer = 1;
						
						EmitSignal(SignalName.teleport, pos);
						EmitSignal(SignalName.dialogueEnd);
					}
				}
				catch(KeyNotFoundException){
					pointer = 1;
					EmitSignal(SignalName.dialogueEnd);
				}
			}
		}
	}
	
}
