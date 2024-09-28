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
	public bool wasAFK;
	public bool locked = false;
	[Export] public double AFKTimer = 60;

	private SpringArm3D springArm;
	private Area3D area3D;
	private AnimationPlayer animPlayer;
	private Godot.Timer timer;
	private Control UI;
	private TextureRect textBox;
	private TextureProgressBar healthBar;
	public static RichTextLabel dialogue;
	[Signal] 
	public delegate void dialogueCommandEventHandler(string name, string command);
	public override void _Ready()
	{
		springArm = GetNode<SpringArm3D>("SpringArm3D");
		Input.MouseMode = Input.MouseModeEnum.Captured;
		animPlayer = GetNode<AnimationPlayer>("charlie2/AnimationPlayer");
		UI = GetParent().GetNode<Control>("UI");
		textBox = UI.GetNode<TextureRect>("TextBox");
		dialogue = textBox.GetNode<RichTextLabel>("Text");
		healthBar = UI.GetNode<TextureProgressBar>("Char1_Health");
		healthBar.Value = health;
		dialogue.VisibleRatio = 0;
		area3D = GetNode<Area3D>("Area3D");
		
		timer = new Godot.Timer();
		AddChild(timer);
		timer.Timeout += onTimeout;

		timer.WaitTime = AFKTimer;
		timer.OneShot = true;
		timer.Start();
	}
	public override void _Input(InputEvent e)
	{
		if(e is InputEventMouseMotion){
			if(wasAFK){
				springArm.Rotation = new Vector3(Mathf.DegToRad(-15),0,0);
				wasAFK=false;
			}
			timer.WaitTime = AFKTimer;
			timer.Start();
			InputEventMouseMotion m = (InputEventMouseMotion) e;
				
			RotateY(Mathf.DegToRad(-m.Relative.X*sensitivityHorizontal));
			float rotVert = Mathf.DegToRad(-m.Relative.Y*sensitivityVertical);
			springArm.RotateX(rotVert);
			springArm.Rotation = springArm.Rotation.Clamp((float)Mathf.DegToRad(-90),(float)Mathf.DegToRad(22.5));
		}
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

		Vector2 inputDir = Input.GetVector("left", "right", "foward", "back");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero&& !locked)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
			animPlayer.Play("Walk");
			timer.WaitTime =10;
			timer.Start();
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
			
			animPlayer.Play("Idle");
			if(timer.IsStopped())
			{
				camSpin();
			}
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
	public void onTimeout()
	{
		wasAFK=true;
	}
	public void endDialogue(){
		locked=false;
		textBox.Visible = false;
		dialogue.VisibleRatio = 0;
	}
	public void camSpin()
	{
		springArm.Rotation = new Godot.Vector3(Mathf.DegToRad(-25),springArm.Rotation.Y,springArm.Rotation.Z);
		springArm.RotateY(Mathf.DegToRad(-0.25f));
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
