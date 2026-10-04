using Godot;
using System;
using System.Collections;
using System.Xml.Resolvers;

public partial class SongLevel : Node2D
{
	//Some variables
	double nextNoteLeft = 0;
	double nextNoteRight = 0;

	// Don't mess with these
	Random random = new Random();
	PackedScene notescene = GD.Load<PackedScene>("res://entities/note.tscn");
	Node2D notes;

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

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{	
		// 
		SpawnNewNotes((float) delta);
	}
}
