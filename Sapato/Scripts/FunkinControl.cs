using Godot;
using Newtonsoft.Json;
using System;
using System.IO;
using FileAccess = Godot.FileAccess;

public partial class FunkinControl : Node
{
	public NoteInfo[] notesList = new NoteInfo[]{};

	public override void _Ready()
	{
		LoadMetaFromPath($"res://Sapato/Songs/{GlobalVariables.CurrentSong}/Meta.json");
		LoadMusic();
		LoadChartFromPath($"res://Sapato/Songs/{GlobalVariables.CurrentSong}/Chart/{GlobalVariables.CurrentDifficult}.json");

		GetNode<AudioStreamPlayer>("../NoteController/MusicControl/Voices").Play();
		GetNode<AudioStreamPlayer>("../NoteController/MusicControl/Inst").Play();

		GetNode<NoteController>("../NoteController").ListNotes = notesList;
		GetNode<NoteController>("../NoteController").GenerateNotes();

		GlobalVariables.HUD = GetNode<CanvasLayer>("../../../HUD");
		GlobalVariables.CamGame = GetNode<Camera2D>("../../../CamGame");

		GetNode<ChartMusicControl>("../NoteController/MusicControl").MusicBeat += (o) => {GlobalVariables.Signals.EmitSignal(GlobalSignals.SignalName.OnMusicBeat, o);};
		GetNode<ChartMusicControl>("../NoteController/MusicControl").MusicStep += (o) => {GlobalVariables.Signals.EmitSignal(GlobalSignals.SignalName.OnMusicStep, o);};
		GetNode<ChartMusicControl>("../NoteController/MusicControl").MusicBar += (o) => {GlobalVariables.Signals.EmitSignal(GlobalSignals.SignalName.OnMusicSection, o);};

		GetNode<ChartMusicControl>("../NoteController/MusicControl").MusicBeat += (o) =>
		{
			if (o % GlobalVariables.CameraShakeBeats != 0)
			{
				return;
			}

			GlobalVariables.HUD.Scale = new Vector2((float)GlobalVariables.HUDBaseZoom+0.05f, (float)GlobalVariables.HUDBaseZoom+0.05f);
			GlobalVariables.CamGame.Zoom = new Vector2((float)GlobalVariables.CameraGAMEBaseZoom+0.03f, (float)GlobalVariables.CameraGAMEBaseZoom+0.03f);
		};
	}

	private void LoadMetaFromPath(string o)
	{
		if (File.Exists(ProjectSettings.GlobalizePath(o)))
		{
			try
			{
				using var file = FileAccess.Open(o, FileAccess.ModeFlags.Read);
				ChartMeta chartmeta = JsonConvert.DeserializeObject<ChartMeta>(file.GetAsText());
				GlobalVariables.BPM = chartmeta.BPM;
				GetNode<ChartMusicControl>("../NoteController/MusicControl").BPM = chartmeta.BPM;
			}
			catch(Exception ex)
			{
				GD.PrintErr($"Can't load the Meta file! {ex}");
			}

		}
	}

	private void LoadMusic()
	{
		try
		{
			if (File.Exists(ProjectSettings.GlobalizePath($"res://Sapato/Songs/{GlobalVariables.CurrentSong}/Music/Voices.ogg")))
			{
				AudioStreamOggVorbis data = AudioStreamOggVorbis.LoadFromFile($"res://Sapato/Songs/{GlobalVariables.CurrentSong}/Music/Voices.ogg");
				GetNode<AudioStreamPlayer>("../NoteController/MusicControl/Voices").Stream = data;
			}
			else if (File.Exists(ProjectSettings.GlobalizePath($"res://Sapato/Songs/{GlobalVariables.CurrentSong}/Music/Voices.mp3")))
			{
				using var file = FileAccess.Open($"res://Sapato/Songs/{GlobalVariables.CurrentSong}/Music/Voices.mp3", FileAccess.ModeFlags.Read);
				var mp3Data = new AudioStreamMP3()
				{
					Data = file.GetBuffer((long)file.GetLength()),
				};
				GetNode<AudioStreamPlayer>("../NoteController/MusicControl/Voices").Stream = mp3Data;
			}

			if (File.Exists(ProjectSettings.GlobalizePath($"res://Sapato/Songs/{GlobalVariables.CurrentSong}/Music/Inst.ogg")))
			{
				AudioStreamOggVorbis data = AudioStreamOggVorbis.LoadFromFile($"res://Sapato/Songs/{GlobalVariables.CurrentSong}/Music/Inst.ogg");
				GetNode<AudioStreamPlayer>("../NoteController/MusicControl/Inst").Stream = data;
			}
			else if (File.Exists(ProjectSettings.GlobalizePath($"res://Sapato/Songs/{GlobalVariables.CurrentSong}/Music/Inst.mp3")))
			{
				using var file = FileAccess.Open($"res://Sapato/Songs/{GlobalVariables.CurrentSong}/Music/Inst.mp3", FileAccess.ModeFlags.Read);
				var mp3Data = new AudioStreamMP3()
				{
					Data = file.GetBuffer((long)file.GetLength()),
				};
				GetNode<AudioStreamPlayer>("../NoteController/MusicControl/Inst").Stream = mp3Data;
			}
			
			if (GetNode<AudioStreamPlayer>("../NoteController/MusicControl/Inst").Stream == null)
			{
				GD.PrintErr($"Cant Load Music Stream Data!! This Files exist?");
			}
		}
		catch (Exception ex)
		{
			GD.PrintErr($"Cant load music data files!! Ex:{ex}");
		}
	}

	private void LoadChartFromPath(string o)
	{
		try
		{
			using var file = FileAccess.Open(o, FileAccess.ModeFlags.Read);
			ChartInfo chartinfo = JsonConvert.DeserializeObject<ChartInfo>(file.GetAsText());
			notesList = chartinfo.notes;
		}
		catch (Exception ex)
		{
			GD.PrintErr($"Cant load chart! Ex: {ex}");
		}
	}

	public override void _Process(double delta)
	{
		GlobalVariables.HUD.Scale = new Vector2(Mathf.Lerp(GlobalVariables.HUD.Scale.X, (float)GlobalVariables.HUDBaseZoom, 5f/(float)(1/delta)), Mathf.Lerp(GlobalVariables.HUD.Scale.Y, (float)GlobalVariables.HUDBaseZoom, 5f/(float)(1/delta)));
		GlobalVariables.CamGame.Zoom = new Vector2(Mathf.Lerp(GlobalVariables.CamGame.Zoom.X, (float)GlobalVariables.CameraGAMEBaseZoom, 5f/(float)(1/delta)), Mathf.Lerp(GlobalVariables.CamGame.Zoom.Y, (float)GlobalVariables.CameraGAMEBaseZoom, 5f/(float)(1/delta)));
		GetNode<NoteController>("../NoteController").ScrollSpeed = GlobalVariables.ScrollSpeed;
	}
}
