using Godot;
using System;

public partial class DetectInteraction : Area3D
{
	public Control UI;
	public TextureRect textBox;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		UI = GetParent().GetParent().GetNode<Control>("UI");
		textBox = UI.GetNode<TextureRect>("TextBox");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}
	public void onBodyEntered(Node3D body)
	{
		if(body.EditorDescription == "Npc")
		{
			GD.Print("Player is looking at an Npc");
			GetParent().SetMeta("canInteract", true);
		}
	}
	public void onBodyExited(Node3D body)
	{
		if(body.EditorDescription == "Npc")
		{
			GD.Print("Player is no longer looking at an Npc");
			GetParent().SetMeta("canInteract", false);
			textBox.Visible = false;
			
		}
	}
}
