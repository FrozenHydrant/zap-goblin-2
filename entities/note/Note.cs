using Godot;
using System;

public partial class Note : Node2D
{
	// Called when the node enters the scene tree for the first time.
	SongLevel mySongLevel;
	public bool leftSide = false;
	public bool powerNote = false;
	int hitWindow;
	// Hitbox is at bottom of note
	const float spriteOffset = 135/2.0f;
	int barLocation;

	[Export]
	public Texture2D noteYellow; 

	[Export]
	public Sprite2D mySprite;

	Vector2 myFallSpeed;
	public override void _Ready()
	{
		hitWindow = SongLevel.hitWindow;
		barLocation = ScoreBar.drawLoc;
		mySongLevel = (SongLevel) GetNode("/root/SongLevel");
		myFallSpeed = SongLevel.fallVec;
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
		Position += myFallSpeed * (float) delta;

		// Delete when outta the screen
		if (Position.Y > 1180) {
			mySongLevel.HitNote(leftSide, SongLevel.Evaluations.VeryLate);
			QueueFree();
		}

	}
}
