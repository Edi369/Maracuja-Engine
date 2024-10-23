using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using FileAccess = Godot.FileAccess;
using System.IO;

public partial class ChartEditor : Node2D
{
	[Export] public float test = 2.987f;
	[Export] public float test2 = 363.817f;
	[Export] public float test3 = 2160.55f;

	private string SongName = "Test";
	private string Difficulty = "hor";

	private float FakeYPosition = 0;
	private Vector2 MouseClickPosition = new Vector2(0, 0);
	private float MouseClickSongPosition = 0;
	private bool Paused = true;
	private float PausedPosition = 0;
	private float ScrollChange = 0;
	private int MouseNoteDirection = -1;
	private float CAMERAZoom = 1;
	private bool IsMusicDragScroll = false;
	private float MusicVolume = 100f;
	private float VoicesVolume = 100f;
	private float MetronomeVolume = 30f;
	private bool IsTesting = false;
	private bool Ocupped = false;
	private float[] Hitsounds = new float[]{ 50f, 50f };
	public static float MusicPosition = 1f;

	public static AudioStreamPlayer Music;
	public static AudioStreamPlayer Voices;
	private Parallax2D TilesBG;
	private Parallax2D BeatBars;
	private Parallax2D BarsBar;
	private Node2D NotesNode;
	private Node InstanceTesting;

	public static List<NoteInfo> ListNotes = new List<NoteInfo>();
	public static List<NoteInfo> NotesSelected = new List<NoteInfo>();
	public static ChartInfo chartinfo;

	public override void _Ready()
	{
		TilesBG = GetNode<Parallax2D>("TilesBackGround");
		BeatBars = GetNode<Parallax2D>("BeatBars");
		BarsBar = GetNode<Parallax2D>("BarsBar");
		NotesNode = GetNode<Node2D>("Notes");

		Music = GetNode<ChartMusicControl>("MusicControl").Music;
		Voices = GetNode<ChartMusicControl>("MusicControl").Voices;

		DisplayServer.WindowSetTitle("FNF - Maracuja Engine (ChartEditor)");

		ListNotes.Clear();
		Events();
		LoadMusic();
		LoadChartFromPath($"res://Sapato/Songs/{SongName}/Chart/{Difficulty}.json");

		ScrollChange = 60f/GetNode<ChartMusicControl>("MusicControl").BPM;
	}

	private void LoadMusic()
	{
		try
		{
			AudioStreamOggVorbis data = new AudioStreamOggVorbis();
			
			if (AudioStreamOggVorbis.LoadFromFile($"res://Sapato/Songs/{SongName}/Music/Inst.ogg") != null)
			{
				data = AudioStreamOggVorbis.LoadFromFile($"res://Sapato/Songs/{SongName}/Music/Inst.ogg");
				Music.Stream = data;

			}

			if (AudioStreamOggVorbis.LoadFromFile($"res://Sapato/Songs/{SongName}/Music/Voices.ogg") != null)
			{
				data = AudioStreamOggVorbis.LoadFromFile($"res://Sapato/Songs/{SongName}/Music/Voices.ogg");
				Voices.Stream = data;
			}
			data = null;

			if (Music.Stream == null)
			{
				using var file = FileAccess.Open($"res://Sapato/Songs/{SongName}/Music/Inst.mp3", FileAccess.ModeFlags.Read);
				var mp3Data = new AudioStreamMP3();
				mp3Data.Data = file.GetBuffer((long)file.GetLength());
				Music.Stream = mp3Data;
			}

			if (Voices.Stream == null)
			{
				using var file = FileAccess.Open($"res://Sapato/Songs/{SongName}/Music/Voices.mp3", FileAccess.ModeFlags.Read);
				var mp3Data = new AudioStreamMP3();
				mp3Data.Data = file.GetBuffer((long)file.GetLength());
				Voices.Stream = mp3Data;
			}
			
			if (Music.Stream == null)
			{
				GD.PrintErr($"Cant Load Music Stream Data!! This Files exist?");
			}
		}
		catch (Exception ex)
		{
			GD.PrintErr($"Cant load music data files!! Ex:{ex}");
		}
	}

	public override void _PhysicsProcess (double delta)
	{
		if (!IsTesting && !Ocupped)
		{
			MousePlaceNote();	
			UpdatePosition();
			LongNoteCreate();
		}

		UpdateText();
		ChartTesting();
	}

	public override void _Process(double delta)
	{
		if (Paused && !IsTesting)
		{
			//FakeYPosition = (-Music.GetPlaybackPosition())*(GetNode<ChartMusicControl>("MusicControl").BPM*test)+test2;
			FakeYPosition = Mathf.Lerp(FakeYPosition, (-Music.GetPlaybackPosition())*(GetNode<ChartMusicControl>("MusicControl").BPM*test)+test2, 10f/(float)(1/delta));
		}
		else if (IsTesting)
		{
			FakeYPosition = (-InstanceTesting.GetNode<ChartMusicControl>("Control/NoteController/MusicControl").GetChild<AudioStreamPlayer>(0).GetPlaybackPosition())*(GetNode<ChartMusicControl>("MusicControl").BPM*test)+test2;
		}
		else
		{
			FakeYPosition = (-Music.GetPlaybackPosition())*(GetNode<ChartMusicControl>("MusicControl").BPM*test)+test2;
		}

		TilesBG.ScrollOffset = new Vector2(TilesBG.ScrollOffset.X, FakeYPosition);
		BeatBars.ScrollOffset = new Vector2(BeatBars.ScrollOffset.X, FakeYPosition);
		BarsBar.ScrollOffset = new Vector2(BarsBar.ScrollOffset.X, FakeYPosition);
		NotesNode.Position = new Vector2(NotesNode.Position.X, FakeYPosition);

		GetNode<Camera2D>("Camera").Zoom = new Vector2(Mathf.Lerp(GetNode<Camera2D>("Camera").Zoom.X, CAMERAZoom, 10f/(float)(1/delta)), Mathf.Lerp(GetNode<Camera2D>("Camera").Zoom.Y, CAMERAZoom, 10f/(float)(1/delta)));
	
		GetNode<TextureRect>("HUDinGame/UPLabel/Metronome/TabContainer/Metronome/MetronomeBall").Modulate = new Color(1, 1, 1, Mathf.Lerp(GetNode<TextureRect>("HUDinGame/UPLabel/Metronome/TabContainer/Metronome/MetronomeBall").Modulate.A, 0f, 10f/(float)(1/delta)));
	}

	private void Events()
	{
		GetNode<FileDialog>("HUDinGame/InfoDataAll/TabContainer/File/SaveJson").Canceled += () =>
		{
			Ocupped = false;
		};

		GetNode<FileDialog>("HUDinGame/InfoDataAll/TabContainer/File/LoadJson").Canceled += () =>
		{
			Ocupped = false;
		};
		
		GetNode<FileDialog>("HUDinGame/InfoDataAll/TabContainer/File/SaveJson").FileSelected += (o) =>
		{
			Ocupped = false;
			try
			{
				File.WriteAllText(o, JsonConvert.SerializeObject(chartinfo));
			}
			catch (Exception ex)
			{
				GD.PrintErr(ex);
			}
		};

		GetNode<FileDialog>("HUDinGame/InfoDataAll/TabContainer/File/LoadJson").FileSelected += (o) =>
		{
			Ocupped = false;
			LoadChartFromPath(o);
		};

		GetNode<HSlider>("HUDEditor/DOWNLabel/TempControl").DragStarted += () => {
			Paused = true;
			PausedPosition = Music.GetPlaybackPosition();
			IsMusicDragScroll = true;
		};

		GetNode<HSlider>("HUDinGame/InfoDataMusic/TabContainer/Audio/Instrumental").ValueChanged += (o) =>
		{
			MusicVolume = (float)o;
			GetNode<Label>("HUDinGame/InfoDataMusic/TabContainer/Audio/Instrumental/InstrumentalVolume").Text = $"Instrumental: {MusicVolume}%";
		};

		GetNode<HSlider>("HUDinGame/InfoDataMusic/TabContainer/Audio/Voices").ValueChanged += (o) =>
		{
			VoicesVolume = (float)o;
			GetNode<Label>("HUDinGame/InfoDataMusic/TabContainer/Audio/Voices/VoicesVolume").Text = $"Voices: {VoicesVolume}%";
		};

		GetNode<HSlider>("HUDinGame/InfoDataMusic/TabContainer/Audio/Dad").ValueChanged += (o) =>
		{
			Hitsounds[0] = (float)o;
			GetNode<Label>("HUDinGame/InfoDataMusic/TabContainer/Audio/Dad/DadHitsoundVolume").Text = $"Dad: {Hitsounds[0]}%";
		};

		GetNode<HSlider>("HUDinGame/InfoDataMusic/TabContainer/Audio/Bf").ValueChanged += (o) =>
		{
			Hitsounds[1] = (float)o;
			GetNode<Label>("HUDinGame/InfoDataMusic/TabContainer/Audio/Bf/BfHitsoundVolume").Text = $"Bf: {Hitsounds[1]}%";
		};

		GetNode<HSlider>("HUDinGame/InfoDataMusic/TabContainer/Audio/Metronome").ValueChanged += (o) =>
		{
			MetronomeVolume = (float)o;
			GetNode<Label>("HUDinGame/InfoDataMusic/TabContainer/Audio/Metronome/MetronomeVolume").Text = $"Metronome: {MetronomeVolume}%";
		};

		GetNode<HSlider>("HUDinGame/UPLabel/VelocitySlider").ValueChanged += (o) =>
		{
			Music.PitchScale = (float)o;
			Voices.PitchScale = (float)o;
		};

		GetNode<HSlider>("HUDEditor/DOWNLabel/TempControl").DragEnded += (o) => IsMusicDragScroll = false;

		GetNode<HSlider>("HUDEditor/DOWNLabel/TempControl").ValueChanged += (o) =>
		{
			if (!IsMusicDragScroll)
			{
				return;
			}
			PausedPosition = (float)o;
			Music.Play((float)o);
			VoicesPlay();
		};

		GetNode<ChartMusicControl>("MusicControl").MusicBeat += (o) => 
		{
			if (Paused)
			{
				return;
			}
			GetNode<TextureRect>("HUDinGame/UPLabel/Metronome/TabContainer/Metronome/MetronomeBall").Modulate = new Color(1, 1, 1, MetronomeVolume/100);
			GetNode<AudioStreamPlayer>("MusicControl/BpmBarSound").VolumeDb = (float)Mathf.LinearToDb(MetronomeVolume/10f);
			GetNode<AudioStreamPlayer>("MusicControl/BpmBarSound").Play();
		};

		GetNode<Button>("HUDinGame/InfoDataAll/TabContainer/File/SaveChart").Pressed += () =>
		{
			Ocupped = true;
			SaveChart();
		};

		GetNode<Button>("HUDinGame/InfoDataAll/TabContainer/File/LoadChart").Pressed += () =>
		{
			Ocupped = true;
			LoadChart();
		};
	}

	void SetupCharPlayMusic()
	{
		InstanceTesting.GetNode<NoteController>("Control/NoteController").ListNotes = ListNotes;
		InstanceTesting.GetNode<NoteController>("Control/NoteController").GenerateNotes();

		ChartMusicControl musicControl = InstanceTesting.GetNode<ChartMusicControl>("Control/NoteController/MusicControl");
		AudioStreamPlayer audioPlayer = musicControl.GetChild<AudioStreamPlayer>(0);
		AudioStreamPlayer voicesPlayer = musicControl.GetChild<AudioStreamPlayer>(1);

		audioPlayer.Stream = Music.Stream;
		voicesPlayer.Stream = Voices.Stream;
		audioPlayer.PitchScale = Music.PitchScale;
		voicesPlayer.PitchScale = Music.PitchScale;
		audioPlayer.Play(MusicPosition);

		if (voicesPlayer.Stream != null && Music.GetPlaybackPosition() < voicesPlayer.Stream.GetLength())
		{
			voicesPlayer.Play(MusicPosition);
		}
	}

	private void ChartTesting()
	{
		if (Input.IsActionJustPressed("CHARTTEST_KEY") && !Ocupped)
		{
			if (IsTesting)
			{
				CallDeferred("remove_child", InstanceTesting);
				IsTesting = false;
				return;
			}

			Paused = true;
			PausedPosition = Music.GetPlaybackPosition();
			Music.StreamPaused = true;
			Voices.StreamPaused = true;
			IsTesting = true;
			PackedScene charplayScene = (PackedScene)ResourceLoader.Load("res://Sapato/Menus/Editor/ChartPlay.tscn");
			InstanceTesting = charplayScene.Instantiate();

			CallDeferred("add_child", InstanceTesting);
			CallDeferred("SetupCharPlayMusic");	
		}
	}
	
	private void UpdateText()
	{
		TimeSpan timeNow = TimeSpan.FromSeconds(Music.GetPlaybackPosition());
		TimeSpan timeLength = TimeSpan.FromSeconds(Music.Stream.GetLength());
		GetNode<HSlider>("HUDEditor/DOWNLabel/TempControl").MaxValue = (float)Music.Stream.GetLength()-0.08f;
		if (!IsMusicDragScroll)
		{
			GetNode<HSlider>("HUDEditor/DOWNLabel/TempControl").Value = Music.GetPlaybackPosition();
		}
		
		GetNode<Label>("HUDEditor/DOWNLabel/MusicTimeNow").Text = $"{((int)timeNow.TotalMinutes < 10 ? $"0{(int)timeNow.TotalMinutes}" : (int)timeNow.TotalMinutes)}:{((int)timeNow.Seconds < 10 ? $"0{(int)timeNow.Seconds}" : (int)timeNow.Seconds)}.{timeNow.Milliseconds}";
		GetNode<Label>("HUDEditor/DOWNLabel/MusicTimeLength").Text = $"{((int)timeLength.TotalMinutes < 10 ? $"0{(int)timeLength.TotalMinutes}" : (int)timeLength.TotalMinutes)}:{((int)timeLength.Seconds < 10 ? $"0{(int)timeLength.Seconds}" : (int)timeLength.Seconds)}.{timeLength.Milliseconds}";
		GetNode<Label>("HUDEditor/DOWNLabel/Infolayer/InfoBeats").Text = $"BPM: {GetNode<ChartMusicControl>("MusicControl").BPM} Beat: {ChartMusicControl.CurrentBPM} Step: {ChartMusicControl.CurrentSTEP} Section: {ChartMusicControl.CurrentBAR}";

		GetNode<ColorRect>("Test").Position = new Vector2(GetNode<ColorRect>("Test").Position.X, FakeYPosition);
	}

	private void UpdatePosition()
	{
		MusicPosition = Music.GetPlaybackPosition();

		if (Paused)
		{
			Music.StreamPaused = true;
			Voices.StreamPaused = true;
			Music.VolumeDb = -255;
			Voices.VolumeDb = -255;
		}
		else
		{
			Music.StreamPaused = false;
			if (Voices.Stream != null && Music.GetPlaybackPosition() < Voices.Stream.GetLength())
			{
				Voices.StreamPaused = false;
			}
			Music.VolumeDb = (float)Mathf.LinearToDb(MusicVolume/100f);
			Voices.VolumeDb = (float)Mathf.LinearToDb(VoicesVolume/100f);
		}

		if (Music.GetPlaybackPosition() > Music.Stream.GetLength()-0.04f)
		{
				Paused = true;
				Music.Play((float)Music.Stream.GetLength()-0.08f);
				//FakeYPosition = (-Music.GetPlaybackPosition()+0.95f)*(GetNode<ChartMusicControl>("MusicControl").BPM*test);
		}

		if (Input.IsActionJustPressed("CHARTZOOMUP_SCROLL"))
		{
			CAMERAZoom += .1f;
		}

		if (Input.IsActionJustPressed("CHARTZOOMDOWN_SCROLL"))
		{
			CAMERAZoom -= .1f;
		}

		if (Input.IsActionJustPressed("CHARTDOWN_SCROLL") && !Input.IsActionJustPressed("CHARTZOOMDOWN_SCROLL"))
		{
			float position = 0;
			if (Paused)
			{
				position = PausedPosition+ScrollChange;
			}
			else
			{
				position = Music.GetPlaybackPosition()+ScrollChange;
			}

			if (position > (float)Music.Stream.GetLength())
			{
				position = (float)Music.Stream.GetLength()-0.08f;
			}

			Music.Play(position);
			VoicesPlay();
			Paused = true;
			PausedPosition = Music.GetPlaybackPosition();
		}

		if (Input.IsActionJustPressed("CHARTUP_SCROLL") && !Input.IsActionJustPressed("CHARTZOOMUP_SCROLL"))
		{
			float position = 0;
			if (Paused)
			{
				position = PausedPosition-ScrollChange;
			}
			else
			{
				position = Music.GetPlaybackPosition()-ScrollChange;
			}

			if (position < 0f)
			{
				position = 0f;
			}

			Music.Play(position);
			VoicesPlay();
			Paused = true;
			PausedPosition = Music.GetPlaybackPosition();
		}

		if (Input.IsActionJustPressed("ui_home"))
		{
			Music.Play(0f);
			VoicesPlay();
			Paused = true;
			PausedPosition = Music.GetPlaybackPosition();
		}

		if (Input.IsActionJustPressed("ui_end"))
		{
			Music.Play((float)Music.Stream.GetLength()-0.08f);
			VoicesPlay();
			Paused = true;
			PausedPosition = Music.GetPlaybackPosition();
		}

		if (Input.IsActionJustPressed("ui_down"))
		{
			float position = 0;
			if (Paused)
			{
				position = PausedPosition+(ScrollChange*4f);
			}
			else
			{
				position = Music.GetPlaybackPosition()+(ScrollChange*4f);
			}

			if (position > (float)Music.Stream.GetLength())
			{
				position = (float)Music.Stream.GetLength()-0.08f;
			}

			Music.Play(position);
			VoicesPlay();
			Paused = true;
			PausedPosition = Music.GetPlaybackPosition();
		}

		if (Input.IsActionJustPressed("CHARTPAUSE_KEY"))
		{
			if (IsTesting)
			{
				return;
			}

			Paused = !Paused;
			if (Paused)
			{
				PausedPosition = Music.GetPlaybackPosition();
			}
			else
			{
				Music.Play(PausedPosition);
				VoicesPlay();
			}
		}
	}

	private bool VoicesPlay()
	{
		if (Voices.Stream == null)
		{
			return false;
		}

		if (Music.GetPlaybackPosition() > Voices.Stream.GetLength())
		{
			return false;
		}

		Voices.Play(Music.GetPlaybackPosition());
		return true;
	}

	private float GetMusicPosition(float positionY)
	{
		return positionY/(GetNode<ChartMusicControl>("MusicControl").BPM*test);
	}

	/// =++++++++++++++++++=
	///   LongNotes Logic
	/// =++++++++++++++++++=

	private void LongNoteCreate()
	{
		ColorRect LongNoteDummy = GetNode<ColorRect>("Notes/NoteDummy/NoteHold");

		if(Input.IsActionJustPressed("CHARTLONGUP_KEY"))
		{
			foreach (NoteInfo note in NotesSelected)
			{
				if (note.LongNote == null)
				{
					note.LongNote = new LongNoteInfo()
					{
						StartTime = note.TimeNote,
						EndTime = note.TimeNote+10f/GetNode<ChartMusicControl>("MusicControl").BPM
					};

					ColorRect longnote = new ColorRect();
					longnote.Size = new Vector2(LongNoteDummy.Size.X, ((note.LongNote.EndTime-note.LongNote.StartTime)*test3)+19.12f);
					longnote.Color = GetColorFromDirection(note.Direction);
					longnote.Position = new Vector2(-12.795f, 61.905f);
					longnote.ZIndex = -1;

					note.Note.AddChild(longnote);
				}
				else
				{
					note.LongNote.EndTime += 10f/GetNode<ChartMusicControl>("MusicControl").BPM;
					UpdateLongNote();
				}
			}
		}

		if(Input.IsActionJustPressed("CHARTLONGDOWN_KEY"))
		{
			foreach (NoteInfo note in NotesSelected)
			{
				if (note.LongNote != null)
				{
					if (note.LongNote.EndTime-.3f <= note.LongNote.StartTime)
					{
						note.Note.GetChild<ColorRect>(0).QueueFree();
						note.Note.RemoveChild(note.Note.GetChild<ColorRect>(0));
						note.LongNote = null;
						continue;
					}

					note.LongNote.EndTime -= 10f/GetNode<ChartMusicControl>("MusicControl").BPM;
					UpdateLongNote();
				}
			}
		}
	}

	public Color GetColorFromDirection(int direction)
	{
		switch (direction)
		{
			case 0: return new Color(0.761f, 0.294f, 0.6f);
			case 1: return new Color(0, 1, 1);
			case 2: return new Color(0.063f, 0.984f, 0);
			case 3: return new Color(0.976f, 0.224f, 0.247f);
			default: return new Color(0f, 0f, 0f);
		}
	}

	public void UpdateLongNote()
	{
		foreach(NoteInfo note in ListNotes.Where(n => n.LongNote != null))
		{
			note.Note.GetChild<ColorRect>(0).Size = new Vector2(note.Note.GetChild<ColorRect>(0).Size.X, ((note.LongNote.EndTime-note.LongNote.StartTime)*test3)+19.12f);
		}
	}

	/// =++++++++++++++++++=
	/// 	Notes Logic
	/// =++++++++++++++++++=

	public void SetMouseDirection()
	{
		if (GetLocalMousePosition().X < 461 || GetLocalMousePosition().X > 820)
		{
			MouseNoteDirection = -1;
			return;
		}

		else if (GetLocalMousePosition().X > 461 && GetLocalMousePosition().X < 505)
		{
			MouseNoteDirection = 0;
		}

		else if (GetLocalMousePosition().X > 505 && GetLocalMousePosition().X < 550)
		{
			MouseNoteDirection = 1;
		}

		else if (GetLocalMousePosition().X > 550 && GetLocalMousePosition().X < 595)
		{
			MouseNoteDirection = 2;
		}

		else if (GetLocalMousePosition().X > 595 && GetLocalMousePosition().X < 640)
		{
			MouseNoteDirection = 3;
		}

		else if (GetLocalMousePosition().X > 642 && GetLocalMousePosition().X < 685)
		{
			MouseNoteDirection = 4;
		}

		else if (GetLocalMousePosition().X > 685 && GetLocalMousePosition().X < 730)
		{
			MouseNoteDirection = 5;
		}

		else if (GetLocalMousePosition().X > 730 && GetLocalMousePosition().X < 775)
		{
			MouseNoteDirection = 6;
		}

		else if (GetLocalMousePosition().X > 775 && GetLocalMousePosition().X < 830)
		{
			MouseNoteDirection = 7;
		}
	}

	private float GetXDirection(int direction)
	{
		switch (direction)
		{
			case 0:
				return 482.875f;

			case 1:
				return 528f;

			case 2:
				return 572.875f;

			case 3:
				return 617.875f;

			case 4:
				return 663f;

			case 5:
				return 708f;

			case 6:
				return 752.5f;

			case 7:
				return 797.5f;

			default:
				return -1000f;
		}
	}

	public void PlaceNote()
	{
		AnimatedSprite2D note = new AnimatedSprite2D();
		note.SpriteFrames = NotesNode.GetNode<AnimatedSprite2D>("NoteDummy").SpriteFrames;
		note.Scale = NotesNode.GetNode<AnimatedSprite2D>("NoteDummy").Scale;
		note.ZIndex = 2;
		
		switch (MouseNoteDirection % 4)
		{
			case 0:
				note.Play("left");
				break;
			case 1:
				note.Play("down");
				break;
			case 2:
				note.Play("up");
				break;
			case 3:
				note.Play("right");
				break;
		}

		note.Position = NotesNode.GetNode<AnimatedSprite2D>("NoteDummy").Position;

		int noteIndex = 0;
		foreach (NoteInfo indexNote in ListNotes)
		{
			noteIndex++;
		}

		NoteInfo info = new NoteInfo
		{
			TimeNote = GetMusicPosition(note.Position.Y),
			Position = note.Position,
			Note = note,
			StrumLine = MouseNoteDirection > 3 ? "Boyfriend" : "Dad",
			NoteType = 0,
			IsPlayAnimation = true,
			Direction = MouseNoteDirection % 4,
			NoteId = noteIndex,
			LongNote = null
		};
		ListNotes.Add(info);
		NotesSelected.Clear();
		NotesSelected.Add(info);

		NotesNode.AddChild(note);
	}

	public string GetStrumOnMouse()
	{
		if (GetLocalMousePosition().X > 461 && GetLocalMousePosition().X < 640)
		{
			return "Dad";
		}

		else if (GetLocalMousePosition().X > 642 && GetLocalMousePosition().X < 820)
		{
			return "Boyfriend";
		}

		return "none";
	}

	private void MousePlaceNote()
	{
		SetMouseDirection();

		foreach(NoteInfo noteinfo in ListNotes)
		{
			if (noteinfo.TimeNote <= Music.GetPlaybackPosition())
			{
				if (noteinfo.Note.Modulate != new Color(0.5f, 0.5f, 0.5f) && noteinfo.Note.Modulate != new Color(0.7f, 0.7f, 0.7f) && !Paused)
				{
					AudioStreamPlayer hitsound = new AudioStreamPlayer();
					hitsound.Stream = GetNode<AudioStreamPlayer>("MusicControl/Hitsound").Stream;
					hitsound.Name = (noteinfo.StrumLine == "Dad" ? "Dad" : "Boyfriend");

					foreach (Node node in GetNode<AudioStreamPlayer>("MusicControl/Hitsound").GetChildren().Where(n => n.Name == hitsound.Name))
					{
						if (node is AudioStreamPlayer epicaudio)
						{
							epicaudio.Stop();
							epicaudio.QueueFree();
							GetNode<AudioStreamPlayer>("MusicControl/Hitsound").RemoveChild(epicaudio);
						}
					}

					GetNode<AudioStreamPlayer>("MusicControl/Hitsound").AddChild(hitsound);
					hitsound.VolumeDb = (float)Mathf.LinearToDb(Hitsounds[(noteinfo.StrumLine == "Dad" ? 0 : 1)]/40f);;
					hitsound.Finished += () => {
						hitsound.QueueFree();
						GetNode<AudioStreamPlayer>("MusicControl/Hitsound").RemoveChild(hitsound);
					}; 
					hitsound.Play();
				}

				if (NotesSelected.Contains(noteinfo))
				{
					noteinfo.Note.Modulate = new Color(0.7f, 0.7f, 0.7f);
				}
				else
				{
					noteinfo.Note.Modulate = new Color(0.5f, 0.5f, 0.5f);
				}
			}
			else
			{
				if (NotesSelected.Contains(noteinfo))
				{
					noteinfo.Note.Modulate = new Color(2.5f, 2.5f, 2.5f);
				}
				else
				{
					noteinfo.Note.Modulate = new Color(1f, 1f, 1f);
				}
			}
		}

		if (MouseNoteDirection == -1)
		{
			NotesNode.GetNode<AnimatedSprite2D>("NoteDummy").Visible = false;
			return;
		}

		NotesNode.GetNode<AnimatedSprite2D>("NoteDummy").Visible = true;
		float NoteXPosition = 0f;
		NoteXPosition = GetXDirection(MouseNoteDirection);
		switch (MouseNoteDirection % 4)
		{
			case 0:
				NotesNode.GetNode<AnimatedSprite2D>("NoteDummy").Play("left");
				break;
			case 1:
				NotesNode.GetNode<AnimatedSprite2D>("NoteDummy").Play("down");
				break;
			case 2:
				NotesNode.GetNode<AnimatedSprite2D>("NoteDummy").Play("up");
				break;
			case 3:
				NotesNode.GetNode<AnimatedSprite2D>("NoteDummy").Play("right");
				break;
		}

		if (Input.IsActionPressed("CHARTSHIFT_KEY"))
		{
			NotesNode.GetNode<AnimatedSprite2D>("NoteDummy").Position = new Vector2(NoteXPosition, GetLocalMousePosition().Y-FakeYPosition);
		}
		else
		{
			NotesNode.GetNode<AnimatedSprite2D>("NoteDummy").Position = new Vector2(NoteXPosition, (Mathf.Floor(((GetLocalMousePosition().Y+52.8f)-FakeYPosition) / 44.8f) * 44.8f)-31.5f);
		}

		if (Input.IsActionJustPressed("MOUSE_CLICK"))
		{
			if (NotesAboveMouse().Count > 0 && !Input.IsActionPressed("CHARTSHIFT_KEY"))
			{
				int _index = 0;
				foreach (NoteInfo note in NotesAboveMouse().OrderBy(n => -n.NoteId))
				{
					_index++;
					if (_index > 1)
					{
						continue;
					}

					NotesSelected.Clear();
					NotesSelected.Add(note);
				}
			}
			else
			{
				PlaceNote();
			}
		}

		if (Input.IsActionPressed("MOUSE_CLICK2"))
		{
			foreach (NoteInfo noteDelete in NotesAboveMouse())
			{
				ListNotes.Remove(noteDelete);
				if (NotesSelected.Contains(noteDelete))
				{
					NotesSelected.Remove(noteDelete);
				}
				Tween _tween1 = CreateTween();
				Tween _tween2 = CreateTween();
				_tween2.TweenProperty(noteDelete.Note, "modulate", new Color(1f, 0f, 0f, 0f), 1f).SetTrans(Tween.TransitionType.Quint).SetEase(Tween.EaseType.Out);
				_tween1.TweenProperty(noteDelete.Note, "scale", Vector2.Zero, 3f).SetTrans(Tween.TransitionType.Quint).SetEase(Tween.EaseType.Out);
				_tween1.TweenCallback(Callable.From(() => {
					NotesNode.RemoveChild(noteDelete.Note);
					noteDelete.Note.QueueFree();
				}));
			}
		}
	}

	public List<NoteInfo> NotesAboveMouse()
	{
		List<NoteInfo> notesabove = new List<NoteInfo>();
		foreach (NoteInfo noteAbove in ListNotes.ToList())
		{
			SetMouseDirection();
			if(noteAbove.StrumLine == GetStrumOnMouse() && MouseNoteDirection % 4 == noteAbove.Direction && noteAbove.Position.Y - (GetLocalMousePosition().Y-FakeYPosition) <= 15f && noteAbove.Position.Y - (GetLocalMousePosition().Y-FakeYPosition) >= -15f)
			{
				notesabove.Add(noteAbove);
			}
		}

		return notesabove;
	}

	/// =++++++++++++++++++=
	/// 	Chart File Logic
	/// =++++++++++++++++++=

	private void LoadChart()
	{
		GetNode<FileDialog>("HUDinGame/InfoDataAll/TabContainer/File/LoadJson").Visible = true;
	}

	private void LoadChartFromPath(string o)
	{
		try
		{
			using var file = FileAccess.Open(o, FileAccess.ModeFlags.Read);
			chartinfo = JsonConvert.DeserializeObject<ChartInfo>(file.GetAsText());

			ColorRect LongNoteDummy = GetNode<ColorRect>("Notes/NoteDummy/NoteHold");
			ListNotes.Clear();
			foreach (Node node in GetNode<Node2D>("Notes").GetChildren())
			{
				if (node.Name == "NoteDummy")
				{
					continue;
				}
				node.QueueFree();
				GetNode<Node2D>("Notes").RemoveChild(node);
			}

			ListNotes = chartinfo.notes.ToList();

			int _notesIndex = 0;
			foreach (NoteInfo noteinfo in ListNotes)
			{
				_notesIndex++;
				AnimatedSprite2D note = new AnimatedSprite2D();
				note.SpriteFrames = NotesNode.GetNode<AnimatedSprite2D>("NoteDummy").SpriteFrames;
				note.Scale = NotesNode.GetNode<AnimatedSprite2D>("NoteDummy").Scale;
				note.ZIndex = 2;

				switch (noteinfo.Direction % 4)
				{
					case 0:
						note.Play("left");
						break;
					case 1:
						note.Play("down");
						break;
					case 2:
						note.Play("up");
						break;
					case 3:
						note.Play("right");
						break;
				}

				note.Position = noteinfo.Position = new Vector2(GetXDirection(noteinfo.Direction+(noteinfo.StrumLine == "Boyfriend" ? 4 : 0)), noteinfo.TimeNote*(GetNode<ChartMusicControl>("MusicControl").BPM*test));

				noteinfo.Note = note;
				noteinfo.NoteId = _notesIndex;

				if (noteinfo.LongNote != null)
				{
					ColorRect longnote = new ColorRect();
					longnote.Size = new Vector2(LongNoteDummy.Size.X, ((noteinfo.LongNote.EndTime-noteinfo.LongNote.StartTime)*test3)+19.12f);
					longnote.Color = GetColorFromDirection(noteinfo.Direction);
					longnote.Position = new Vector2(-12.795f, 61.905f);
					longnote.ZIndex = -1;

					note.AddChild(longnote);
				}

				NotesNode.AddChild(note);
			}
		}
		catch (Exception ex)
		{
			GD.PrintErr(ex);
		}
	}

	private void SaveChart()
	{
		chartinfo = new ChartInfo()
		{
			dif = Difficulty,
			notes = ListNotes.ToArray(),
			even = "",
			InitialTime = DateTime.Now,
		};

		GetNode<FileDialog>("HUDinGame/InfoDataAll/TabContainer/File/SaveJson").Visible = true;
	}
}