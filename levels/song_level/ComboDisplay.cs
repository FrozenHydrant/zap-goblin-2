using Godot;
using System;

public partial class ComboDisplay : Node2D
{
	[Export]
	RichTextLabel leftComboLabel;
	[Export]
	RichTextLabel rightComboLabel;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		leftComboLabel.Text = "0";
		rightComboLabel.Text = "0";
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void UpdateComboDisplay(int comboLeft, int comboRight)
	{
		leftComboLabel.Text = comboLeft.ToString();
		rightComboLabel.Text = comboRight.ToString();
	}

}
