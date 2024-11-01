using Godot;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public partial class NoteController : Node
{
	private float SongPositionY = 0f;
	public List<NoteInfo> ListNotes = new List<NoteInfo>();
	public Dictionary<int, NoteInfo> ActualNote = new Dictionary<int, NoteInfo>();
	public Dictionary<int, LongInfo> ActualLongs = new Dictionary<int, LongInfo>();
	private AnimatedSprite2D NoteDummy;
	private PackedScene LongNoteDummy;
	private AnimatedSprite2D LongNoteEndDummy;
	private ChartMusicControl MusicControl;
	public int NotePlayerIndex = 0;
	public double ScrollSpeed = 1;
	public bool[] StrumsPressed = new bool[]{ false, false, false, false };

	public override void _Ready()
	{
		GlobalVariables.StrumLines.Clear();

		NoteDummy = GetNode<AnimatedSprite2D>("NoteDummy");
		MusicControl = GetNode<ChartMusicControl>("MusicControl");
		LongNoteDummy = (PackedScene)ResourceLoader.Load("res://Sapato/Objects/Game/LongNote.tscn");

		foreach (Node strum in GetParent().GetChildren())
		{
			if (!strum.Name.ToString().StartsWith("STRUM"))
			{
				continue;
			}

			GlobalVariables.StrumLines.Add(
				strum.GetChild<Node2D>(0).Name.ToString().Replace("STRUM", ""),
				new Strum{
				LEFT = strum.GetChild<Node2D>(0).GetChild<AnimatedSprite2D>(0),
				DOWN = strum.GetChild<Node2D>(0).GetChild<AnimatedSprite2D>(1),
				UP = strum.GetChild<Node2D>(0).GetChild<AnimatedSprite2D>(2),
				RIGHT = strum.GetChild<Node2D>(0).GetChild<AnimatedSprite2D>(3),
				STRUM = strum.GetChild<Strums>(0),
			});
		}
	}

	public void GenerateNotes()
	{
		int noteIndex = 0;
		foreach (NoteInfo noteinfo in ListNotes.ToList().OrderBy(n => n.TimeNote))
		{
			noteIndex++;

			if (!GlobalVariables.StrumLines.ContainsKey(noteinfo.StrumLine))
			{
				GD.PrintErr("Strum Not Found!");
				continue;
			}

			if (noteinfo.TimeNote < ChartEditor.Music.GetPlaybackPosition())
			{
				continue;
			}

			AnimatedSprite2D note = new AnimatedSprite2D();
			Node2D LongNoteDummyInstace = LongNoteDummy.Instantiate<Node2D>();
			
			note.Name = $"note{noteIndex}";
			LongNoteDummyInstace.Name = $"longnote{noteIndex}";
			note.SpriteFrames = NoteDummy.SpriteFrames;

			switch (noteinfo.Direction)
			{
				default:
					note.Play("left");
					//LongNoteDummyInstace.GetChild(0).GetChild<AnimatedSprite2D>(0).Play($"left hold piece");
					break;
				case 1:
					note.Play("down");
					//LongNoteDummyInstace.GetChild(0).GetChild<AnimatedSprite2D>(0).Play($"down hold piece");
					break;
				case 2:
					note.Play("up");
					//LongNoteDummyInstace.GetChild(0).GetChild<AnimatedSprite2D>(0).Play($"up hold piece");
					break;
				case 3:
					note.Play("right");
					//LongNoteDummyInstace.GetChild(0).GetChild<AnimatedSprite2D>(0).Play($"right hold piece");
					break;
			}

			//NoteInfo info = noteinfo;
			//info.Note = note;
			//ActualNote.Add(noteIndex, info);
			note.Visible = false;
			LongNoteDummyInstace.Visible = false;

			if (noteinfo.LongNoteLenght != null)
			{
				ActualLongs.Add(noteIndex, new LongInfo
				{
					LongInstance = LongNoteDummyInstace,
					Direction = noteinfo.Direction,
					StrumLine = noteinfo.StrumLine,
					//LongEndSprite = null,
					LongNoteStarted = noteinfo.TimeNote,
					LongNoteEnd = (float)noteinfo.LongNoteLenght
				});
			}

			ActualNote.Add(noteIndex, new NoteInfo
			{
				TimeNote = noteinfo.TimeNote,
				Note = note,
				NoteType = noteinfo.NoteType,
				IsPlayAnimation = noteinfo.IsPlayAnimation,
				StrumLine = noteinfo.StrumLine,
				Direction = noteinfo.Direction,
				LongNoteLenght = noteinfo.LongNoteLenght
			});
			
			foreach (Node node in GlobalVariables.StrumLines[noteinfo.StrumLine].STRUM.GetChildren())
			{
				if (node.Name != "Notes")
				{
					continue;
				}

				if (ActualNote[noteIndex].LongNoteLenght != null)
				{
					node.AddChild(LongNoteDummyInstace);
				}

				node.AddChild(note);
			}
		}
	}

	public AnimatedSprite2D GetStrumNote(string strum, int index, out AnimatedSprite2D strumSprite)
	{
		if (!GlobalVariables.StrumLines.ContainsKey(strum))
		{
			strumSprite = null;
			return strumSprite;
		}

		switch (index)
		{
			case 0:
				strumSprite = GlobalVariables.StrumLines[strum].LEFT;
				return strumSprite;
			case 1:
				strumSprite = GlobalVariables.StrumLines[strum].DOWN;
				return strumSprite;
			case 2:
				strumSprite = GlobalVariables.StrumLines[strum].UP;
				return strumSprite;
			case 3:
				strumSprite = GlobalVariables.StrumLines[strum].RIGHT;
				return strumSprite;
		}
		strumSprite = null;
		return strumSprite;
	}

	public override void _Process(double delta)
	{
		foreach(KeyValuePair<int, NoteInfo> note in ActualNote.Where(n => n.Value.TimeNote - MusicControl.Music.GetPlaybackPosition() < .6f))
		{
			if (GetStrumNote(note.Value.StrumLine, note.Value.Direction, out AnimatedSprite2D strumSprite) != null)
			{
				note.Value.Note.Visible = true;
				note.Value.Note.Position = new Vector2(strumSprite.Position.X, (note.Value.TimeNote-MusicControl.GetChild<AudioStreamPlayer>(0).GetPlaybackPosition())*(GetNode<ChartMusicControl>("MusicControl").BPM*(float)(11.948*ScrollSpeed)));
			}
		}
		
		foreach(KeyValuePair<int, LongInfo> longnote in ActualLongs.Where(n => n.Value.LongNoteStarted - MusicControl.Music.GetPlaybackPosition() < .6f))
		{
			if (GetStrumNote(longnote.Value.StrumLine, longnote.Value.Direction, out AnimatedSprite2D strumSprite) != null)
			{
				//longnote.Value.LongInstance.Visible = true;
				//longnote.Value.LongInstance.Size = new Vector2(longnote.Value.LongInstance.Size.X, ((longnote.Value.LongNoteEnd-longnote.Value.LongNoteStarted)*(GetNode<ChartMusicControl>("MusicControl").BPM*(float)(17.7*ScrollSpeed)))+150f);
				//longnote.Value.LongInstance.GetChild<Parallax2D>(0).RepeatTimes = 100;
				//longnote.Value.LongInstance.Position = new Vector2(strumSprite.Position.X-24, (longnote.Value.LongNoteStarted-MusicControl.GetChild<AudioStreamPlayer>(0).GetPlaybackPosition())*(GetNode<ChartMusicControl>("MusicControl").BPM*(float)(11.948*ScrollSpeed)));
				////longnote.Value.LongEndSprite.Position = new Vector2(24.5f, longnote.Value.LongNoteEnd*(GetNode<ChartMusicControl>("MusicControl").BPM*(float)(11.948*ScrollSpeed)));
			}
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		foreach (KeyValuePair<string, Strum> strum in GlobalVariables.StrumLines)
		{
			if (strum.Value.STRUM.Player)
			{
				continue;
			}

			for (int i = 0; i < 4; i++)
			{
				GetStrumNote(strum.Key, i, out AnimatedSprite2D strumSprite);
		
				if (strumSprite == null)
				{
					continue;
				}
		
				if (strumSprite.FrameProgress < 1)
				{
					continue;
				}
		
				strumSprite.Play("idle");
			}
		}

		foreach(KeyValuePair<int, NoteInfo> note in ActualNote.Where(n => n.Value.TimeNote - MusicControl.Music.GetPlaybackPosition() < .15f))
		{
			NoteInfo oldNote = note.Value;

			if (note.Value.TimeNote - MusicControl.Music.GetPlaybackPosition() < -.15f)
			{
				MusicControl.Voices.VolumeDb = -255;
				ActualNote.Remove(note.Key);
				note.Value.Note.QueueFree();
			}

			if (GlobalVariables.StrumLines.ContainsKey(note.Value.StrumLine) && GlobalVariables.StrumLines[note.Value.StrumLine].STRUM.Player)
			{
				PlayerPlayStrum(oldNote, note.Key);
				continue;	
			}

			if (note.Value.TimeNote <= MusicControl.GetChild<AudioStreamPlayer>(0).GetPlaybackPosition())
			{
				DadPlayStrum(oldNote, note.Key);
				continue;
			}
		}

		for (int i = 0; i < 4; i++)
		{
			if (Input.IsActionPressed(GetInputName(i)) && !StrumsPressed[i])
			{
				if (GetStrumNote("Boyfriend", i, out AnimatedSprite2D strumSprite).Animation != "press")
				{
					strumSprite.Play("press");	
				}
			}

			else if (!Input.IsActionPressed(GetInputName(i)))
			{
				GetStrumNote("Boyfriend", i, out AnimatedSprite2D strumSprite).Play("idle");
				StrumsPressed[i] = false;
			}
		}
	}

	public async void PlayStrumAnimation(int index , string StrumLine)
	{
		MusicControl.Voices.VolumeDb = 0;
		GetStrumNote(StrumLine, index, out AnimatedSprite2D strumSprite);

		if (strumSprite == null)
		{
			return;
		}

		await Task.Delay(1); 
		strumSprite.Play("confirm");
	}

	public bool PlayerPlayStrum(NoteInfo note, int index)
	{
		if (!Input.IsActionJustPressed(GetInputName(note.Direction)))
		{
			return false;
		}

		//GetStrumNote(note.StrumLine, note.Direction, out AnimatedSprite2D strumSprite);

		if (StrumsPressed[note.Direction])
		{
			return false;
		}

		PlayStrumAnimation(note.Direction, note.StrumLine);
		ActualNote.Remove(index);
		note.Note.QueueFree();

		foreach (Node node in GlobalVariables.StrumLines[note.StrumLine].STRUM.GetChildren())
		{
			if (node.Name != "Notes")
			{
				continue;
			}

			node.RemoveChild(note.Note);
		}

		NotePlayerIndex++;

		StrumsPressed[note.Direction] = true;
		
		return true;
	}

	public string GetInputName(int direction)
	{
		switch (direction)
		{
			case 0:
				return "LEFT_KEY";
			case 1:
				return "DOWN_KEY";
			case 2:
				return "UP_KEY";
			case 3:
				return "RIGHT_KEY";

			default:
				return "?!?!?!";
		}
	}

	public void DadPlayStrum(NoteInfo note, int index)
	{
		PlayStrumAnimation(note.Direction, note.StrumLine);
		ActualNote.Remove(index);
		note.Note.QueueFree();

		foreach (Node node in GlobalVariables.StrumLines[note.StrumLine].STRUM.GetChildren())
		{
			if (node.Name != "Notes")
			{
				continue;
			}

			node.RemoveChild(note.Note);
		}
	}
}
