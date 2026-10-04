using Godot;
using System;
using System.Collections;
using System.Xml.Resolvers;

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

	// Don't mess with these
	Random random = new Random();
	[Export]
	PackedScene notescene;
	[Export]
	HitDisplay hitDisplay;
	Node2D notes;
	public const int hitWindow = 260;

	// Called when scenetree 1st time
	public override void _Ready()
	{
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

	public void HitNoteDisplay(bool isLeft, Evaluations evaluation) {
		switch (evaluation)
		{
			case Evaluations.Marvelous:
			{
				hitDisplay.FlashText(isLeft, "Marvelous");
				break;
			} 
			case Evaluations.Late:
			{
				hitDisplay.FlashText(isLeft, "Late");	
				break;	
			}
			case Evaluations.Early:
			{
				hitDisplay.FlashText(isLeft, "Early");
				break;
			}
			case Evaluations.VeryEarly:
			{
				hitDisplay.FlashText(isLeft, "Miserably Early");
				break;		
			}
			case Evaluations.VeryLate:
			{
				hitDisplay.FlashText(isLeft, "Miserably Late");
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
				HitNoteDisplay(isLeft, Evaluations.Marvelous);
			}	
			else if (closest < hitWindow/2.0f)
			{
				if (direction == -1.0f)
				{
					HitNoteDisplay(isLeft, Evaluations.Early);
				} else
				{
					HitNoteDisplay(isLeft, Evaluations.Late);
				}
			} else
			{
				if (direction == -1.0f)
				{
					HitNoteDisplay(isLeft, Evaluations.VeryEarly);
				} else
				{
					HitNoteDisplay(isLeft, Evaluations.VeryLate);
				}
			}
		}

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{	
		// 
		SpawnNewNotes((float) delta);
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
