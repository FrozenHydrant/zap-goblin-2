using Godot;
using System;
using System.Diagnostics;

public partial class Healthbar : Node2D
{
	// Called when the node enters the scene tree for the first time.
	[Export]
	float hpBarLength = 300;
	[Export]
	RichTextLabel lifeCountLabel;
	Vector2 hpStartPos;
	Vector2 hpEndPos;
	public override void _Ready()
	{
		hpStartPos = new Vector2(-hpBarLength/2.0f, 0); 
		hpEndPos = new Vector2(hpStartPos.X + hpBarLength, 0);
		UpdateHealthbarDisplay(1.0f);
	}

	public void UpdateHealthDisplay(float percentage, int lives)
	{
		UpdateLivesDisplay(lives);
		UpdateHealthbarDisplay(percentage);
	}

	public void UpdateHealthbarDisplay(float percentage)
	{
		hpEndPos = new Vector2(hpStartPos.X + hpBarLength * percentage, 0);
		QueueRedraw();
	}

	public void UpdateLivesDisplay(int lives)
	{
		lifeCountLabel.Text = "x" + lives.ToString();
	}

	public override void _Draw()
	{
		base._Draw();

		GD.Print(hpStartPos, " ", hpEndPos);
		DrawLine(hpStartPos, hpEndPos, Colors.Green, 15.0f);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
