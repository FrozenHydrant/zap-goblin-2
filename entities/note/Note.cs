using Godot;
using System;

public partial class Note : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public bool leftSide = false;
	int hitWindow;
	[Export]
	public Vector2 fallVec = new Vector2(0.0f, 800.0f);
	// Hitbox is at bottom of note
	const float spriteOffset = 135/2.0f;
	int barLocation;
	public override void _Ready()
	{
		hitWindow = SongLevel.hitWindow;
		barLocation = ScoreBar.drawLoc;
	}
	
	public bool Targetable()
	{
		float dist = GetDistance();
		if (Mathf.Abs(dist) < hitWindow)
		{
			return true;
		}
		return false;
	}

	public float GetDistance()
	{
		return Position.Y - (barLocation - spriteOffset);
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
