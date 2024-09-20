using Godot;
using System;

public partial class CombatController : Node3D
{
	private int[,] map;
	private int [,] defaultOrientation;
	private Vector2 [,] worldPositions;
	private Node3D markers;
	public override void _Ready()
	{
		/*  0: Empty
			1: Party member 1
			2: Party member 2
			3: Party member 3
			4: Enemy 1
			5: Enemy 2
			6: Enemy 3
			7: Enemy 4
			8: Enemy 5
			9: Enemy 6
		*/
		map = new int[,]			   {{0,0,0,0,0,0},
			  			 				{0,0,0,0,0,0},
						 				{0,0,0,0,0,0}};

		defaultOrientation = new int[,]{{1,0,0,0,0,0},
			  			 				{0,0,0,0,0,0},
						 				{0,0,0,0,0,0}};
		
		worldPositions = new Vector2[6,3];

		markers = GetNode<Node3D>("markers");
		for(int i = 0; i<6; i++){
			for(int j = 0; j < 3; j++){
				MeshInstance3D m = markers.GetNode<MeshInstance3D>("("+i+","+j+")");
				worldPositions[i,j] = new Vector2(m.Position.X, m.Position.Z);
				GD.Print(i+","+j+":"+worldPositions[i,j]);
			}
		}

		map = defaultOrientation;
	}

	public override void _Process(double delta)
	{
	}
	public void initiateCombat(){
		Vector3 origin = this.Position;

		
	}
}
