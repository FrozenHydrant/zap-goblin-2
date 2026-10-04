using Godot;
using System;

public partial class SongLevel : Node2D
{
	public enum Evaluations
	{
		VeryLate,
		VeryEarly,
		Marvelous,
		Late,
		Early
	}

	//Some variables
	double nextNoteLeft = 0;
	double nextNoteRight = 0;
	int leftCombo = 0;
	int rightCombo = 0;
	float hp = 100;
	float damage = 25;
	int lives = 2;
	int score = 0;
	int targetScore = 5000;
	float leftComboStrength = 1 / 60f;
	float rightComboStrength = 1 / 60f;
	float powerNoteChance = 0.0f;

	//time related variables
	private float timetracker = 0.0f;
	private const float trackerinterval = 5.0f;

	// Don't mess with these
	Random random = new();
	[Export]
	PackedScene notescene;
	[Export]
	HitDisplay hitDisplay;
	[Export]
	ComboDisplay comboDisplay;
	[Export]
	Healthbar healthbar;
	[Export]
	ScoreDisplay scoreDisplay;
	[Export]
	PackedScene failScene;
	[Export]
	PackedScene succeedScene;
	static GameState State => GameState.Instance;
	const float maxHp = 100;
	Node2D notes;
	public const int hitWindow = 260;
	const int originalTargetScore = 5000;

	public static Vector2 fallVec = new Vector2(0.0f, 800.0f);

	// Called when scenetree 1st time
	public override void _Ready()
	{
		notes = GetNode<Node2D>("Notes");

		ParseAndApplyUpgrades();
		healthbar.UpdateLivesDisplay(lives);

		targetScore = (int) (originalTargetScore * Mathf.Pow(1.1, State.stage));
		scoreDisplay.UpdateTargetScoreDisplay(targetScore);

	}

	public void ParseAndApplyUpgrades()
	{
		foreach (string upgrade in State.skills)
		{
			if (State.Has(upgrade))
			{
				ApplyUpgrade(upgrade);
			}
		}
	}

	public void ApplyUpgrade(string upgrade)
	{
		switch (upgrade)
		{
			case "life1":
				{
					lives += 1;
					break;
				}
			case "combo":
				{
					leftComboStrength *= 1.2f;
					rightComboStrength *= 1.2f;
					break;
				}
			case "life2":
				{
					lives += 1;
					break;
				}
			case "power":
				{
					powerNoteChance = 0.05f;
					break;
				}
			case "power2":
				{
					powerNoteChance = 0.1f;
					break;
				}
			default:
				{
					break;
				}
		}
	}

	public void SpawnNoteLeft()
	{
		Note myNote = (Note)notescene.Instantiate();
		double myDouble = random.NextDouble();
		if (myDouble <= powerNoteChance)
		{
			myNote.powerNote = true;
			myNote.mySprite.Texture = myNote.noteYellow;
		}
		myNote.leftSide = true;
		myNote.Position = new Vector2(700, -100);
		notes.AddChild(myNote);
	}

	public void SpawnNoteRight()
	{
		Note myNote = (Note)notescene.Instantiate();
		double myDouble = random.NextDouble();
		if (myDouble <= powerNoteChance)
		{
			myNote.powerNote = true;
			myNote.mySprite.Texture = myNote.noteYellow;
		}
		myNote.leftSide = false;
		myNote.Position = new Vector2(1100, -100);
		notes.AddChild(myNote);
	}

	public void SpawnNewNotes(double delta)
	{
		// Note Spawning
		nextNoteLeft -= delta;
		if (nextNoteLeft <= 0)
		{
			SpawnNoteLeft();
			nextNoteLeft = random.Next(50, 100) / 100.0f;
			// GD.Print(nextNoteLeft + " delay");
		}
		nextNoteRight -= delta;
		if (nextNoteRight <= 0)
		{
			SpawnNoteRight();
			nextNoteRight = random.Next(50, 100) / 100.0f;
		}
	}

	public void UpdateCombo(bool isLeft, bool reset)
	{
		if (isLeft)
		{
			if (reset)
			{
				leftCombo = 0;
			}
			else
			{
				leftCombo += 1;
			}
		}
		else
		{
			if (reset)
			{
				rightCombo = 0;
			}
			else
			{
				rightCombo += 1;
			}
		}
		comboDisplay.UpdateComboDisplay(leftCombo, rightCombo);
	}

	public void UpdateHp(float damage)
	{
		hp -= damage;
		if (hp <= 0)
		{
			lives -= 1;
			hp = maxHp;
		}
		if (lives < 1)
		{
			GoToFailure();
		}
		healthbar.UpdateHealthDisplay(hp / maxHp, lives);
	}

	public void GoToFailure()
	{
		GetTree().ChangeSceneToPacked(failScene);
	}

	public void GoToSuccess()
	{
		State.SkillPoints += 2;
		State.stage += 1;
		GetTree().ChangeSceneToPacked(succeedScene);
	}
	public void UpdateScore(bool isLeft, int change)
	{
		if (isLeft)
		{
			score += (int)(change * (leftCombo * leftComboStrength));
			if (leftCombo >= 100 && State.Has("multi"))
			{
				score *= 2;
			}
		}
		else
		{
			score += (int)(change * (rightCombo * rightComboStrength));
			if (rightCombo >= 100 && State.Has("multi"))
			{
				score *= 2;
			}
		}
		if (score >= targetScore)
		{
			GoToSuccess();
		}
		scoreDisplay.UpdateScoreDisplay(score);
	}

	public void HitNote(bool isLeft, Evaluations evaluation)
	{
		switch (evaluation)
		{
			case Evaluations.Marvelous:
				{
					hitDisplay.FlashText(isLeft, "Marvelous");
					UpdateCombo(isLeft, false);
					break;
				}
			case Evaluations.Late:
				{
					hitDisplay.FlashText(isLeft, "Late");
					UpdateCombo(isLeft, false);
					break;
				}
			case Evaluations.Early:
				{
					hitDisplay.FlashText(isLeft, "Early");
					UpdateCombo(isLeft, false);
					break;
				}
			case Evaluations.VeryEarly:
				{
					hitDisplay.FlashText(isLeft, "Miserably Early");
					UpdateCombo(isLeft, true);
					UpdateHp(damage);
					break;
				}
			case Evaluations.VeryLate:
				{
					hitDisplay.FlashText(isLeft, "Miserably Late");
					UpdateCombo(isLeft, true);
					UpdateHp(damage);
					break;
				}
			default:
				{
					break;
				}
		}
	}

	public void HandleKeyPress(bool isLeft)
	{
		// We score the note closest to the hit bar when a hit is detected
		float closest = float.MaxValue;
		int direction = 1;
		Note closestNote = null;
		foreach (Note note in notes.GetChildren())
		{
			if (note.leftSide == isLeft && note.Targetable())
			{
				float amount = note.GetDistance();
				float score = Mathf.Abs(amount);

				// Update the closest value & its score
				if (score < closest)
				{
					direction = Mathf.Sign(amount);
					closest = score;
					closestNote = note;
				}
			}
		}
		//GD.Print(closest);

		if (closestNote != null)
		{
			// Do hit calcs
			if (closest < hitWindow / 4.0f)
			{
				HitNote(isLeft, Evaluations.Marvelous);
			}
			else if (closest < hitWindow / 2.0f)
			{
				if (direction == -1.0f)
				{
					HitNote(isLeft, Evaluations.Early);
				}
				else
				{
					HitNote(isLeft, Evaluations.Late);
				}
			}
			else
			{
				if (direction == -1.0f)
				{
					HitNote(isLeft, Evaluations.VeryEarly);
				}
				else
				{
					HitNote(isLeft, Evaluations.VeryLate);
				}
			}

			// Clean up the note
			closestNote.QueueFree();
			int trueScore = (int)(hitWindow - closest);
			if (closestNote.powerNote)
			{
				trueScore *= 2;
			}
			UpdateScore(isLeft, trueScore);
		}

	}

	public void ModifyFallSpeedMultiplicative(float multiplier)
	{
		fallVec *= multiplier;
	}

	public void ModifyFallSpeedAdditive(int amount)
	{
		fallVec.Y += amount;
	}

	public void Speedup(double delta)
	{
		timetracker += (float)delta;
		if (timetracker >= trackerinterval)
		{
			ModifyFallSpeedAdditive(50);
			timetracker -= trackerinterval;
		}

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// 
		SpawnNewNotes((float)delta);
		Speedup(delta);
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		base._UnhandledInput(@event);

		if (@event.IsActionPressed("left_press"))
		{
			HandleKeyPress(true);
		}
		else if (@event.IsActionPressed("right_press"))
		{
			HandleKeyPress(false);
		}
	}

}
