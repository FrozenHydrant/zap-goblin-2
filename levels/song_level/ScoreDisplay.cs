using Godot;
using System;

public partial class ScoreDisplay : Node2D
{
	[Export]
	RichTextLabel scoreDisplayText;

	[Export]
	RichTextLabel targetScoreDisplayText;
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

	public void UpdateTargetScoreDisplay(int targetScore)
	{
		targetScoreDisplayText.Text = "Target: " + targetScore.ToString();
	}
}
