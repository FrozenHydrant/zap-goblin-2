using Godot;
using System;

public partial class Note : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public bool leftSide = false;
	[Export]
	public Vector2 fallVec = new Vector2(0.0f, 800.0f);
	public override void _Ready()
	{
			 
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Position += fallVec * (float) delta;

		// Delete when outta the screen
		if (Position.Y > 1180) {
			QueueFree();
		}
	}
}
