using Godot;
using System;
using System.Linq;
using System.Threading;

public partial class Player : CharacterBody3D
{
	public float Speed = 5.0f;
	public float JumpVelocity = 4.5f;
	private float health = 100f;
	[Export] public float sensitivityHorizontal = 0.5f;
	[Export] public float sensitivityVertical = 0.5f;
	public bool locked = false;
	private Area3D area3D;
	private Camera3D cam;
	private AnimationPlayer animPlayer;
	private Control UI;
	private TextureRect textBox;
	private TextureProgressBar healthBar;
	public static RichTextLabel dialogue;
	[Signal] 
	public delegate void dialogueCommandEventHandler(string name, string command);
	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;
		animPlayer = GetNode<AnimationPlayer>("charlie2/AnimationPlayer");
		UI = GetParent().GetNode<Control>("UI");
		textBox = UI.GetNode<TextureRect>("TextBox");
		dialogue = textBox.GetNode<RichTextLabel>("Text");
		healthBar = UI.GetNode<TextureProgressBar>("Char1_Health");
		cam = GetParent().GetNode<Camera3D>("Camera3D");
		healthBar.Value = health;
		dialogue.VisibleRatio = 0;
		area3D = GetNode<Area3D>("Area3D");
		RotateY(cam.Rotation.Y);
	}
	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;
		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		if (Input.IsActionJustPressed("jump") && IsOnFloor() && !locked)
		{
			velocity.Y = JumpVelocity;
		}

		if (Input.IsActionJustPressed("escape"))
		{
			Input.MouseMode = Input.MouseModeEnum.Visible;
		}

		if (Input.IsActionJustPressed("interact") && GetMeta("canInteract").AsBool())
		{
			if(!locked && dialogue.VisibleRatio != 1)
			{

				GD.Print("The player interacted with something");
				EmitSignal(SignalName.dialogueCommand,area3D.GetOverlappingAreas().Last().GetParent().GetMeta("name"),"begin");
				dialogue.VisibleRatio = 0;
				textBox.Visible = true;
				locked = true;
			}
			else if(locked && dialogue.VisibleRatio != 1){
				dialogue.VisibleRatio = 1;
			}
			else if(locked && dialogue.VisibleRatio == 1)
			{
				EmitSignal(SignalName.dialogueCommand,area3D.GetOverlappingAreas().Last().GetParent().GetMeta("name"),"next");
				dialogue.VisibleRatio = 0;
			}
			else if(!locked && dialogue.VisibleRatio == 1){
				locked = false;
				textBox.Visible = false;
				dialogue.VisibleRatio = 0;
			}
		}

		if (Input.IsActionPressed("sprint") && !locked)
		{
			Speed = 10.0f;
		}
		else{
			Speed = 5.0f;
		}

		Vector2 inputDir = Input.GetVector("left", "right", "forward", "back");
		Vector3 direction = cam.Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y).Normalized();
		if (direction != Vector3.Zero&& !locked)
		{
			GD.Print("X: "+ direction.X+ " | Y: "+ direction.Y+ " | Z: "+ direction.Z);
			this.Rotation = new Vector3(0,(float)Math.Atan2(-direction.X,-direction.Z),0);
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
			animPlayer.Play("Walk");
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
			
			animPlayer.Play("Idle");
		}

		Velocity = velocity;
		MoveAndSlide();
	}
	public override void _Process(double delta)
	{
		if(locked && dialogue.VisibleRatio != 1){
			dialogue.VisibleRatio += (float)(0.2 * delta);
		}
	}
	public void endDialogue(){
		locked=false;
		textBox.Visible = false;
		dialogue.VisibleRatio = 0;
	}
	
	//getters and setters
	public float getHealth(){
		return health;
	}
	public void setHealth(float h){
		health = h;
		healthBar.Value = h;
	}
	public void teleport(Vector3 pos){
		this.GlobalPosition = pos;
	}
	public void Lock(){
		locked = true;
	}
}
