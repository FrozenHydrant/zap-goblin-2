using Godot;
using System.Collections.Generic;
using System.Linq;

// Attach to a root Control node. Builds the whole tree in code.
// Character sprites: res://art/characters/<id>.png  (stormy.png, sparky.png, ...)
// Click a locked node to buy it.
// Unlocked character: left-click picks for whoever the toggle says (P1/P2), right-click always picks for P2.
// Picking the other player's character swaps them.
public partial class SkillTree : Control
{
	private const string GameScene = "res://skill_tree.tscn";   // <- change to your game scene
	private const string CharArtDir = "res://characters/";

	private static readonly Color Teal    = Color.FromHtml("#31AAA9");
	private static readonly Color Cream   = Color.FromHtml("#F8E0A4");
	private static readonly Color DarkRed = Color.FromHtml("#6C1A1A");
	private static readonly Color Ink     = Color.FromHtml("#1C1C20");

	private record Skill(string Id, string Name, string Desc, int Cost, Vector2 Pos,
						 string[] Parents, bool Character = false);

	// "Parents" = unlocking ANY one parent makes the node available.
	private static readonly Skill[] Skills =
	{
		// --- root + starter characters ---
		new("core",    "Start",       "Where it begins.",                                  0, new(605, 590), new string[0]),
		new("stormy",  "Stormy",      "Steady Storm: +1 life every song.",                 0, new(470, 600), new[] { "core" }, true),
		new("sparky",  "Sparky",      "Live Wire: +15% score.",                            0, new(740, 600), new[] { "core" }, true),

		// --- tier 1 ---
		new("power",   "Power Notes", "Power notes appear. Worth 2x score.",               1, new(473, 475), new[] { "core" }),
		new("life1",   "+1 Life",     "Start each song with one extra shared life.",       1, new(740, 482), new[] { "core" }),
		new("spendy",  "Spendy",      "Big Spender: earn 50% more skill points.",          2, new(300, 520), new[] { "power" }, true),

		// --- tier 2 ---
		new("combo",   "Combo Boost", "Score multiplier grows faster with combo.",         2, new(435, 356), new[] { "power" }),
		new("freeze",  "Time Freeze", "Once per song, freeze the notes for 2 seconds.",    2, new(595, 356), new[] { "power" }),
		new("life2",   "+1 Life",     "Another extra shared life.",                        2, new(722, 344), new[] { "life1" }),
		new("trendy",  "Trendy",      "Trendsetter: combo multiplier builds 2x as fast.",  3, new(250, 330), new[] { "combo" }, true),
		new("oswald",  "Oswald",      "Old Soul: heals 1 life every 100 combo.",           3, new(900, 410), new[] { "life2" }, true),

		// --- tier 3 ---
		new("power2",  "Overcharge",  "Power notes appear twice as often.",                3, new(251, 220), new[] { "combo" }),
		new("shield",  "Shield",      "Your first miss each song doesn't count.",          3, new(346, 177), new[] { "combo" }),
		new("multi",   "Frenzy",      "x2 score while combo is above 100.",                3, new(471, 170), new[] { "combo" }),
		new("sync",    "In Sync",     "Bonus when both players hit at the same moment.",   3, new(641, 200), new[] { "freeze", "life2" }),
		new("lenient", "Soft Touch",  "Wider timing window for the touch-sensor player.",  3, new(901, 195), new[] { "life2" }),
		new("stacky",  "Stacky",      "Stack Overflow: each 300 in a row adds +0.1x score (max 3x). A miss resets it.", 4, new(120, 250), new[] { "power2" }, true),
		new("shady",   "Shady",       "Lights Out: notes fade before the hit line, but 1.5x score.", 4, new(860, 290), new[] { "sync" }, true),

		// --- capstone ---
		new("sleepy",  "Sleepy",      "Snooze Button: once per song, when you'd lose your last life, time freezes and you get it back.",
																						   6, new(605, 60), new[] { "power2", "shield", "multi", "sync", "lenient" }, true),
	};

	private static readonly Vector2 SkillSize = new(110, 36);
	private static readonly Vector2 CharSize  = new(72, 84);

	private static readonly Color P1Color = Color.FromHtml("#F8E0A4");   // cream border
	private static readonly Color P2Color = Color.FromHtml("#A82020");   // red border

	private readonly Dictionary<string, Button> _buttons = new();
	private Label _pointsLabel, _p1Label, _p2Label;
	private Button _pickToggle;
	private int _picking = 1;

	private static GameState State => GameState.Instance;

	public override void _Ready()
	{
		TextureFilter = TextureFilterEnum.Nearest;   // crisp pixel art (children inherit)
		
		_pointsLabel = MakeLabel(new Vector2(20, 16));
		_p1Label     = MakeLabel(new Vector2(20, 40));
		_p2Label     = MakeLabel(new Vector2(20, 64));

		foreach (var s in Skills)
		{
			var size = s.Character ? CharSize : SkillSize;
			var b = new Button
			{
				Text = s.Name,
				TooltipText = $"{s.Desc}\nCost: {s.Cost}",
				CustomMinimumSize = size,
				Position = s.Pos - size / 2,
			};
			string id = s.Id;
			if (s.Character)
			{
				string path = CharArtDir + s.Id + ".png";
				if (ResourceLoader.Exists(path))
					b.Icon = GD.Load<Texture2D>(path);
				b.ExpandIcon = true;
				b.IconAlignment = HorizontalAlignment.Center;
				b.VerticalIconAlignment = VerticalAlignment.Top;

				// right-click = pick for P2
				b.GuiInput += ev =>
				{
					if (ev is InputEventMouseButton mb && mb.Pressed && mb.ButtonIndex == MouseButton.Right
						&& State.Has(id))
					{
						Pick(2, id);
						Refresh();
					}
				};
			}
			b.Pressed += () => OnPressed(id);
			AddChild(b);
			_buttons[id] = b;
		}

		_pickToggle = new Button
		{
			CustomMinimumSize = new Vector2(120, 40),
			Position = new Vector2(1010, 530),
		};
		_pickToggle.Pressed += () => { _picking = _picking == 1 ? 2 : 1; Refresh(); };
		AddChild(_pickToggle);

		var play = new Button
		{
			Text = "Play",
			CustomMinimumSize = new Vector2(120, 44),
			Position = new Vector2(1010, 580),
		};
		play.Pressed += () => GetTree().ChangeSceneToFile(GameScene);
		AddChild(play);

		Refresh();
	}

	private Label MakeLabel(Vector2 pos)
	{
		var l = new Label { Position = pos };
		l.AddThemeColorOverride("font_color", Cream);
		AddChild(l);
		return l;
	}

	private bool CanUnlock(Skill s) =>
		!State.Has(s.Id)
		&& State.SkillPoints >= s.Cost
		&& (s.Parents.Length == 0 || s.Parents.Any(p => State.Has(p)));

	private void OnPressed(string id)
	{
		var s = Skills.First(k => k.Id == id);

		if (State.Has(id))
		{
			if (s.Character) Pick(_picking, id);
		}
		else if (CanUnlock(s))
		{
			State.SkillPoints -= s.Cost;
			State.Unlock(id);
		}
		Refresh();
	}

	// Picking the character the other player has swaps them.
	private static void Pick(int player, string id)
	{
		if (player == 1)
		{
			if (State.P2Character == id) State.P2Character = State.P1Character;
			State.P1Character = id;
		}
		else
		{
			if (State.P1Character == id) State.P1Character = State.P2Character;
			State.P2Character = id;
		}
	}

	private static string Short(Skill s)
	{
		int colon = s.Desc.IndexOf(':');
		return colon > 0 ? $"{s.Name} ({s.Desc[..colon]})" : s.Name;
	}

	private void Refresh()
	{
		_pointsLabel.Text = $"Skill points: {State.SkillPoints}";
		_p1Label.Text = "P1: " + Short(Skills.First(k => k.Id == State.P1Character));
		_p2Label.Text = "P2: " + Short(Skills.First(k => k.Id == State.P2Character));
		_pickToggle.Text = $"Picking: P{_picking}";
		Style(_pickToggle, _picking == 1 ? P1Color : P2Color, Ink, null);

		foreach (var s in Skills)
		{
			var b = _buttons[s.Id];
			Color? border = null;
			string tag = "";
			if (s.Character && s.Id == State.P1Character) { border = P1Color; tag = " P1"; }
			if (s.Character && s.Id == State.P2Character) { border = P2Color; tag = " P2"; }
			b.Text = s.Name + tag;

			if (State.Has(s.Id))   Style(b, Teal, Ink, border);                        // unlocked
			else if (CanUnlock(s)) Style(b, Cream, DarkRed, null);                     // available
			else                   Style(b, DarkRed, new Color(Cream, 0.6f), null);    // locked
		}
		QueueRedraw();
	}

	private static void Style(Button b, Color bg, Color text, Color? selectedBorder)
	{
		StyleBoxFlat Box(Color c)
		{
			int w = selectedBorder.HasValue ? 4 : 2;
			return new StyleBoxFlat
			{
				BgColor = c,
				BorderColor = selectedBorder ?? c.Darkened(0.45f),
				BorderWidthLeft = w, BorderWidthTop = w, BorderWidthRight = w, BorderWidthBottom = w,
			};
		}
		b.AddThemeStyleboxOverride("normal", Box(bg));
		b.AddThemeStyleboxOverride("hover", Box(bg.Lightened(0.15f)));
		b.AddThemeStyleboxOverride("pressed", Box(bg.Darkened(0.15f)));
		b.AddThemeStyleboxOverride("focus", new StyleBoxEmpty());
		foreach (var c in new[] { "font_color", "font_hover_color", "font_pressed_color", "font_focus_color" })
			b.AddThemeColorOverride(c, text);
		// Locked characters show as a dark silhouette until bought.
		b.AddThemeColorOverride("icon_normal_color", bg == DarkRed ? Colors.Black : Colors.White);
		b.AddThemeColorOverride("icon_hover_color", bg == DarkRed ? Colors.Black : Colors.White);
	}

	// Lines draw under the buttons because children render on top of _Draw.
	public override void _Draw()
	{
		DrawRect(new Rect2(Vector2.Zero, GetViewportRect().Size), Ink);
		foreach (var s in Skills)
			foreach (var pid in s.Parents)
			{
				var parent = Skills.First(k => k.Id == pid);
				bool lit = State.Has(s.Id) && State.Has(pid);
				DrawLine(parent.Pos, s.Pos, lit ? Teal : new Color(Cream, 0.25f), 3);
			}
	}
}
