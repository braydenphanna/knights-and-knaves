using Godot;
using Godot.Collections;
using Godot.NativeInterop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

public partial class NpcBehavior : CharacterBody3D
{
	[Export] string name;
	int pointer = 1;
	public AnimationPlayer animPlayer;
	Json json;
	RichTextLabel text;
	
	[Signal] public delegate void dialogueEndEventHandler(string command);

	public override void _Ready()
	{
		animPlayer = GetNode<AnimationPlayer>("character3/AnimationPlayer");
		animPlayer.Play("Idle");
		SetMeta("name", name);

		text = GetParent().GetNode<RichTextLabel>("UI/TextBox/Text");

		Godot.FileAccess file = Godot.FileAccess.Open("res://dialogue/area1.json", Godot.FileAccess.ModeFlags.Read);
		json = new Json();
		Error dialogue = json.Parse(file.GetAsText(),true);
	}
	public void dialogueCommandReceiver(string name, string command)
	{
		GD.Print(name);
		if(name == this.name){
			if(command == "begin")
			{
				text.Text= json.Data.AsGodotDictionary()[name].AsGodotDictionary()[pointer.ToString()].AsGodotDictionary()["text"].ToString();
			}
			else if(command == "next")
			{
				pointer++;
				try{
					text.Text= json.Data.AsGodotDictionary()[name].AsGodotDictionary()[pointer.ToString()].AsGodotDictionary()["text"].ToString();
				}
				catch(KeyNotFoundException){
					EmitSignal(SignalName.dialogueEnd);
					pointer = 1;
				}
			}
		}
	}
	
}
