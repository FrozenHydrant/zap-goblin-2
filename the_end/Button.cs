using Godot;
using System;

public partial class Button : Godot.Button
{
	const string retryLocation = "res://skill_tree.tscn";
	static GameState State => GameState.Instance;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Pressed += () => GoToStart();
	}

	public void GoToStart()
	{
		GetTree().ChangeSceneToFile(retryLocation);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
