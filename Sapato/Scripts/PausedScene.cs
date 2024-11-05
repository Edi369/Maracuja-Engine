using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public partial class PausedScene : CanvasLayer
{
	public Control PausedControl;
	public Control OptionsNode;
	public Node[] Options = Array.Empty<Node>();
	public string[] ChartOptionsString = new string[] { "Exit To Charter", $"Pratice Mode" };
	public string[] OptionsString = new string[] { "Controls", "Options", "Exit To Menu" };
	public Dictionary<int, Vector2> BasePositions = new Dictionary<int, Vector2>();
	public Vector2 OptionNodeBasePosition = new Vector2();
	public AudioStreamPlayer BGPausedMusic;
	public AudioStreamPlayer ScrollSfx;
	public int CurrentSelected = 0;

	public override void _Ready()
	{
		PausedControl = GetNode<Control>("PausedControl");
		OptionsNode = PausedControl.GetNode<Control>("Resume");
		BGPausedMusic = GetNode<AudioStreamPlayer>("BGPausedMusic");
		ScrollSfx = GetNode<AudioStreamPlayer>("ScrollSfx");

		OptionNodeBasePosition = OptionsNode.Position;
		Options = OptionsNode.GetChildren().ToArray();

		if (GlobalVariables.InChartMode)
		{	
			foreach(string optionchart in ChartOptionsString)
			{
				AddOption(optionchart);
			}
		}

		foreach(string optionchart in OptionsString)
		{
			AddOption(optionchart);
		}

		LoadMusicBg($"res://Sapato/Music/GlobalCharsMusics/{GlobalVariables.CurGlobalCharacter}/breakfast.ogg");
	}

	public void LoadMusicBg(string path)
	{
		AudioStreamOggVorbis audioData = AudioStreamOggVorbis.LoadFromFile(path);
		audioData.Loop = true;
		BGPausedMusic.Stream = audioData;
		BGPausedMusic.VolumeDb = -255;
		BGPausedMusic.Play();
	}

	public void PlaySFXSound(string path)
	{
		string finalPath = ProjectSettings.GlobalizePath(path);
		string baseFilename = "keyClick";

		string[] _existsVariations = Directory.GetFiles(finalPath).Where(f => f.StartsWith($"{finalPath}{baseFilename}") && f.EndsWith(".ogg")).ToArray();

		if (_existsVariations.Length <= 0)
		{
			return;
		}

		int indexSound = new RandomNumberGenerator().RandiRange(1, _existsVariations.Length);

		AudioStreamOggVorbis audioData = AudioStreamOggVorbis.LoadFromFile($"{finalPath}{baseFilename}{indexSound}.ogg");
		ScrollSfx.Stream = audioData;
		ScrollSfx.Play();
	}

	public void AddOption(string text)
	{
		AlphabeticScript optionChartNode = ResourceLoader.Load<PackedScene>("res://Sapato/Objects/Game/AlphabeticCene.tscn").Instantiate<AlphabeticScript>();
		optionChartNode.FnfText = text;
		optionChartNode.Name = text;
		if (Options.Length > 0 && Options[Options.Length-1] is Node2D lastOption)
		{
			optionChartNode.Position = new Vector2(lastOption.Position.X+11, lastOption.Position.Y+144);
		}
		else
		{
			optionChartNode.Position = new Vector2(22, 32);
		}

		OptionsNode.AddChild(optionChartNode);
		Options = OptionsNode.GetChildren().ToArray();

		BasePositions.Clear();
		foreach(Node option in Options)
		{
			if (option is Node2D letter)
			{
				BasePositions.Add(option.GetIndex(), letter.Position);
			}
		}
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("ui_up") || Input.IsActionJustPressed("CHARTUP_SCROLL"))
		{
			CurrentSelected--;
			PlaySFXSound("res://Sapato/Sounds/");
			if (CurrentSelected < 0)
			{
				CurrentSelected = Options.Length-1;
			}
		}
		else if (Input.IsActionJustPressed("ui_down") || Input.IsActionJustPressed("CHARTDOWN_SCROLL"))
		{
			CurrentSelected++;
			PlaySFXSound("res://Sapato/Sounds/");
			if (CurrentSelected > Options.Length-1)
			{
				CurrentSelected = 0;
			}
		}

		OptionsNode.Position = new Vector2(Mathf.Lerp(PausedControl.GetNode<Control>("Resume").Position.X, 88-BasePositions[CurrentSelected].X, 10f/(float)(1/delta)), Mathf.Lerp(PausedControl.GetNode<Control>("Resume").Position.Y, OptionNodeBasePosition.Y+(32-BasePositions[CurrentSelected].Y), 10f/(float)(1/delta)));

		foreach (Node option in Options)
		{
			if (option is Node2D letter)
			{
				if (option.GetIndex() == CurrentSelected)
				{
					letter.Modulate = new Color(letter.Modulate.R, letter.Modulate.G, letter.Modulate.B, 1);
					letter.Position = new Vector2(Mathf.Lerp(letter.Position.X, BasePositions[option.GetIndex()].X+50f, 10f/(float)(1/delta)), BasePositions[option.GetIndex()].Y);
				}
				else
				{
					letter.Modulate = new Color(letter.Modulate.R, letter.Modulate.G, letter.Modulate.B, .5f);
					letter.Position = new Vector2(Mathf.Lerp(letter.Position.X, BasePositions[option.GetIndex()].X, 10f/(float)(1/delta)), BasePositions[option.GetIndex()].Y);
				}

				if (letter.Name == "Pratice Mode")
				{
					if (!GlobalVariables.CanDie)
					{
						letter.Modulate = new Color(1, .989f, .23f, letter.Modulate.A);
					}
					else
					{
						letter.Modulate = new Color(1, 1, 1, letter.Modulate.A);
					}
				}
			}
		}

		if (Input.IsActionJustPressed("ui_accept") || Input.IsActionJustPressed("MOUSE_CLICK"))
		{
			GlobalVariables.Signals.EmitSignal(GlobalSignals.SignalName.OnSeletecPausedOption, Options[CurrentSelected].Name);
			switch(Options[CurrentSelected].Name)
			{
				case "Resume":
					GetTree().Paused = false;
					GlobalVariables.Signals.EmitSignal(GlobalSignals.SignalName.OnResumedSong);
					this.QueueFree();
					GetParent().RemoveChild(this);
				break;
				case "Restart Song":
					GetTree().Paused = false;
					GetTree().ReloadCurrentScene();
				break;
				case "Exit To Charter":
					GetTree().Paused = false;
					ChartEditor.SongName = GlobalVariables.CurrentSong;
					ChartEditor.Difficulty = GlobalVariables.CurrentDifficult;
					GlobalVariables.Signals.EmitSignal(GlobalSignals.SignalName.OnExitSong);
					GetTree().ChangeSceneToFile("res://Sapato/Menus/Editor/ChartEditor.tscn");
				break;
				case "Pratice Mode":
					GlobalVariables.CanDie = !GlobalVariables.CanDie;
				break;
			}
		}

		BGPausedMusic.VolumeDb = Mathf.Lerp(BGPausedMusic.VolumeDb, -20, .6f/(float)(1/delta));
	}
}