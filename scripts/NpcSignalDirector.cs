using Godot;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;

public partial class NpcSignalDirector : Node
{
	private Json json;
	public void dialogueCommandRedirector(string name, string command)
	{
		for(int i = 0; i < GetChildCount(); i++){
			Node child = GetChild(i);
			if(child.GetMeta("name").ToString() == name){
				(child as Npc).dialogueCommandReceiver(command);
			}
		}
	}
	public void spawnNpcs(string world){

		for(int j = 0; j <GetChildCount();j++){
			GetChild(j).QueueFree();
		}

		Godot.FileAccess file = Godot.FileAccess.Open("res://levels/"+world+".json", Godot.FileAccess.ModeFlags.Read);
		json = new Json();
		Error npcInfo = json.Parse(file.GetAsText(),true);
		int i = 1;
		while(true){
			try{
				
				Node3D temp = GD.Load<PackedScene>("res://entities/npc.tscn").Instantiate() as Node3D;
				(temp as Npc).setMesh(json.Data.AsGodotDictionary()[i.ToString()].AsGodotDictionary()["mesh"].ToString());
				
				string[] posRaw = json.Data.AsGodotDictionary()[i.ToString()].AsGodotDictionary()["pos"].ToString().Split(",");
				Vector3 pos = new Vector3(posRaw[0].ToFloat(),posRaw[1].ToFloat(),posRaw[2].ToFloat());
				temp.Position = pos;
				
				string[] rotRaw = json.Data.AsGodotDictionary()[i.ToString()].AsGodotDictionary()["rot"].ToString().Split(",");
				Vector3 rot = new Vector3(Mathf.DegToRad(rotRaw[0].ToFloat()),Mathf.DegToRad(rotRaw[1].ToFloat()),Mathf.DegToRad(rotRaw[2].ToFloat()));
				temp.Rotation = rot;

				AddChild(temp);
				(temp as Npc).init(json.Data.AsGodotDictionary()[i.ToString()].AsGodotDictionary()["name"].ToString());
				i++;
			}
			catch(KeyNotFoundException e){
				break;
			}
		}
	}
}
