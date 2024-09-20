using Godot;
using System;

public partial class NpcSignalDirector : Node
{
	public void dialogueCommandRedirector(string name, string command)
	{
		for(int i = 0; i < GetChildCount(); i++){
			Node child = GetChild(i);
			if(child.GetMeta("name").ToString() == name){
				(child as Npc).dialogueCommandReceiver(command);
			}
		}
	}
}
