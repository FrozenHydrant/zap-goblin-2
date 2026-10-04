using Godot;
using System;

public partial class HitDisplay : Node2D
{
	[Export]
	RichTextLabel hitDisplayLeft;
	[Export]
	RichTextLabel hitDisplayRight;
	// Called when the node enters the scene tree for the first time.
	RichTextLabel[] textLabels;
	const float fadeSpeed = 2.6f;
	public override void _Ready()
	{
		textLabels = [hitDisplayLeft, hitDisplayRight];
	}

	public void FlashText(bool isLeft, string text)
	{
		RichTextLabel myLabel;
		if (isLeft)
		{
			myLabel = hitDisplayLeft;
		} else
		{
			myLabel = hitDisplayRight;
		}

		myLabel.Text = text;
		myLabel.Modulate = Colors.White;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		foreach (RichTextLabel label in textLabels)
		{
			Color labelCol = label.Modulate;
			if (labelCol.A > 0) {
				labelCol.A -= fadeSpeed * (float) delta;
				label.Modulate = labelCol;
			}
		}
	}
}
