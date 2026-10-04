using Godot;
using System;

public partial class GoodEndButton : Button
{
	[Export]
	PackedScene skillTree;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Pressed += () => GoToSkills();
	}

	public void GoToSkills()
	{
		GetTree().ChangeSceneToPacked(skillTree);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
