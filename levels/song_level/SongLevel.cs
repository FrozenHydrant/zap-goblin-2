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
	const float maxHp = 100;
	Node2D notes;
	public const int hitWindow = 260;

	public static Vector2 fallVec = new Vector2(0.0f, 800.0f);

	// Called when scenetree 1st time
	public override void _Ready()
	{
		healthbar.UpdateLivesDisplay(lives);
		notes = GetNode<Node2D>("Notes");
	}

	public void SpawnNoteLeft()
	{
		Note myNote = (Note) notescene.Instantiate();
		myNote.leftSide = true;
		myNote.Position = new Vector2(700, -100);
		notes.AddChild(myNote);
	}

	public void SpawnNoteRight()
	{
		Note myNote = (Note) notescene.Instantiate();
		myNote.leftSide = false;
		myNote.Position = new Vector2(1100, -100);
		notes.AddChild(myNote);
	}

	public void SpawnNewNotes(double delta)
	{
		// Note Spawning
		nextNoteLeft -= delta;	
		if (nextNoteLeft <= 0) {
			SpawnNoteLeft();
			nextNoteLeft = random.Next(50, 100)/100.0f;
			// GD.Print(nextNoteLeft + " delay");
		}
		nextNoteRight -= delta;
		if (nextNoteRight <= 0)
		{
			SpawnNoteRight();
			nextNoteRight = random.Next(50, 100)/100.0f;
		}
	}

	public void UpdateCombo(bool isLeft, bool reset)
	{
		if (isLeft)
		{
			if (reset)
			{
				leftCombo = 0;
			} else
			{
				leftCombo += 1;
			}
		} else
		{
			if (reset)
			{
				rightCombo = 0;
			} else
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
			// TODO
		}
		healthbar.UpdateHealthDisplay(hp / maxHp, lives);
	}

	public void HitNote(bool isLeft, Evaluations evaluation) {
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
			if (note.leftSide == isLeft && note.Targetable()) {
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
			closestNote.QueueFree();
			int trueScore = (int) (hitWindow - closest);

			if (closest < hitWindow/4.0f)
			{
				HitNote(isLeft, Evaluations.Marvelous);
			}	
			else if (closest < hitWindow/2.0f)
			{
				if (direction == -1.0f)
				{
					HitNote(isLeft, Evaluations.Early);
				} else
				{
					HitNote(isLeft, Evaluations.Late);
				}
			} else
			{
				if (direction == -1.0f)
				{
					HitNote(isLeft, Evaluations.VeryEarly);
				} else
				{
					HitNote(isLeft, Evaluations.VeryLate);
				}
			}
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

	public void Speedup(double delta){
		timetracker += (float)delta;
		if (timetracker >= trackerinterval){
			ModifyFallSpeedAdditive(50);
			timetracker -= trackerinterval;
		}

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{	
		// 
		SpawnNewNotes((float) delta);
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
