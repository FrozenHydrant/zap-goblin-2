using Godot;
using System.Collections.Generic;

// Autoload this as "GameState" (Project Settings -> Globals -> Autoload).
// From C#:       GameState.Instance.Has("life1"), GameState.Instance.P1Character
// From GDScript: GameState.Has("life1"), GameState.P1Character / GameState.P2Character, GameState.SkillPoints += 2
public partial class GameState : Node
{
	public static GameState Instance { get; private set; }

	[Export] public int SkillPoints { get; set; } = 5;               // starting points for testing
	[Export] public string P1Character { get; set; } = "stormy";
	[Export] public string P2Character { get; set; } = "sparky";

	// Stormy and Sparky start unlocked.
	private readonly HashSet<string> _unlocked = new() { "core", "stormy", "sparky" };

	public override void _Ready() => Instance = this;

	public bool Has(string id) => _unlocked.Contains(id);
	public void Unlock(string id) => _unlocked.Add(id);
}
