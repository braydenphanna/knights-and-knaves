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
	private string mesh;
	
	public void init(string n)
	{
		animPlayer = GetNode<AnimationPlayer>(mesh+"/AnimationPlayer");
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
						(GetParent().GetParent()as Main).Tele(npcEvent.Substring(5));
						pointer = 1;
					}
					else if(npcEvent.Substring(0,4) == "wrld"){
						(GetParent().GetParent()as Main).Wrld(npcEvent.Substring(5));
						pointer = 1;
					}
					else if(npcEvent.Substring(0,4) == "cmbt"){
						(GetParent().GetParent()as Main).Cmbt();
						pointer = 1;
					}
				}
				catch(KeyNotFoundException){
					pointer = 1;
					GetParent().GetParent().GetNode<Player>("Player").endDialogue();
				}
			}
		}
	}
	public void setMesh(string m){
		AddChild(GD.Load<PackedScene>("res://assets/"+m+".blend").Instantiate());
		mesh = m;
	}
	
}
