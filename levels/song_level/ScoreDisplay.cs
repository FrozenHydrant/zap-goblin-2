using Godot;
using System;

public partial class ScoreDisplay : Node2D
{
	[Export]
	RichTextLabel scoreDisplayText;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		UpdateScoreDisplay(0);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void UpdateScoreDisplay(int score)
	{
		scoreDisplayText.Text = score.ToString();
	}
}
