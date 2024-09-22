using Godot;
using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

public partial class CombatController : Node
{
	private int[,] map;
	private int [,] defaultOrientation;
	private Vector2 [,] worldPositions;
	private Node3D markers;
	[Export] Node3D tempMesh;
	public void init()
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

		defaultOrientation = new int[,]{{0,0,0,0,0,0},
			  			 				{0,0,0,1,0,0},
						 				{0,0,0,0,0,0}};
		
		worldPositions = new Vector2[3,6];

		markers = GetParent().GetNode<Node3D>("arena/Markers");
		for(int i = 0; i<map.GetLength(0); i++){
			for(int j = 0; j < map.GetLength(1); j++){
				MeshInstance3D m = markers.GetNode<MeshInstance3D>("("+j+","+i+")");
				worldPositions[i,j] = new Vector2(m.Position.X, m.Position.Z);
				//GD.Print(i+","+j+":"+worldPositions[i,j]);
			}
		}
	}
	public void enterCombat(){
		//create Random enemy array and call override with the parameter 
		Enemy[] enemies = new Enemy[1];

		enterCombat(enemies);
	}

	public void enterCombat(Enemy[] enemies){
		Random r = new Random();

		//Places n enemies randomly on blank map
		int e = 9;
		while (9-e<enemies.Length){
			int x= (int)r.NextInt64(map.GetLength(0));
			int y = (int)r.NextInt64(map.GetLength(1));
			if(map[x,y]==0){
				map[x,y] = e;
				e--;
			}
		}

		//places party members
		 for(int i = 0; i<map.GetLength(0); i++){
			for(int j = 0; j < map.GetLength(1); j++){
				//if default position is empty place party member there
				if(defaultOrientation[i,j] > 0 && map[i,j] == 0){
					map[i,j] = defaultOrientation[i,j];
				}
				//if default position is occupied compare strength stats
				else if(defaultOrientation[i,j] >0 && map[i,j] != 0){
					//inplement strength stat comparison

					//---TEMPORARY--- RANDOMLY PLACE PARTY MEMBER
					while (true){
						int x= (int)r.NextInt64(map.GetLength(0));
						int y = (int)r.NextInt64(map.GetLength(1));
						if(map[x,y]==0){
							map[x,y] = defaultOrientation[i,j];
							break;
						}
					}
				}
			}
		}
		//map completed

		//begin placing characters in world
		for(int i = 0; i<map.GetLength(0); i++){
			for(int j = 0; j < map.GetLength(1); j++){
				switch(map[i,j]){
					case 0:
						break;
					case 1:
						Vector2 pos = worldPositions[i,j];
						Player temp  = GetParent().GetNode<Player>("Player");
						temp.Position = new Vector3(pos.X, 1,pos.Y);
						temp.Lock();
						break;
					default:
					GD.Print(map[i,j]+" "+i+" "+j+" "+enemies);
						spawnEnemy(map[i,j],i,j,enemies);
						break;
				}
			}
		}

		print2DArray(map);
	}

	public Vector2 find(int n){
		for(int i = 0; i<map.GetLength(0); i++){
			for(int j = 0; j < map.GetLength(1); j++){
				if(map[i,j] == n){
					return new Vector2(i,j);
				}
			}
		}
		return new Vector2(-1,-1);
	}
	public void print2DArray(int[,] arr){
		for(int i = 0; i<map.GetLength(0); i++){
			string line = "";
			for(int j = 0; j < map.GetLength(1); j++){
				line += arr[i,j]+", ";
			}
			GD.Print(line);
		}
	}
	public void spawnEnemy(int identifier, int i, int j, Enemy[] enemies){
		Vector2 pos = worldPositions[i,j];
		Enemy temp = enemies[9-identifier];
		AddChild(temp);
		temp.Position = new Vector3(pos.X, 0,pos.Y);
		temp.LookAt(GetParent().GetNode<Player>("Player").Position);
	}
}
