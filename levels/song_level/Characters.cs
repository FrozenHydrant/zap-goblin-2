using Godot;
using System;

public partial class Characters : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public static GameState State => GameState.Instance;
	[Export]
	TextureRect leftPlayerTexture;
	[Export]
	TextureRect rightPlayerTexture;
	public override void _Ready()
	{
		string path = SkillTree.CharArtDir + State.P1Character + ".png";
		leftPlayerTexture.Texture = GD.Load<Texture2D>(path);

		string rightPath = SkillTree.CharArtDir + State.P2Character + ".png";
		rightPlayerTexture.Texture = GD.Load<Texture2D>(rightPath);

		GD.Print(path);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
