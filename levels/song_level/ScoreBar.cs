using Godot;
using System;

public partial class ScoreBar : Node2D
{
	public const int drawLoc = 900;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _Draw()
	{
		base._Draw();
		DrawLine(new Vector2(0, drawLoc), new Vector2(1920, drawLoc), Colors.White);
	}

}
