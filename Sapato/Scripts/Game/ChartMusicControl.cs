using Godot;
using System;

public partial class ChartMusicControl : Node
{
	[Export] public AudioStreamPlayer Music { get; set; }
	
	[Export] public AudioStreamPlayer Voices { get; set; }

	[Export] public int BPM = 135;
	
	public static int CurrentBPM = 0;
	private static int _oldBPM = 0;
	public static int CurrentSTEP = 0;
	private static int _oldSTEP = 0;
	public static int CurrentBAR = 0;
	private static int _oldBAR = 0;

	public static bool SongFinished = false;

	[Signal] public delegate void MusicBeatEventHandler(int Beat);
	[Signal] public delegate void MusicStepEventHandler(int Step);
	[Signal] public delegate void MusicBarEventHandler(int Bar);

	public override void _PhysicsProcess(double delta)
	{
		if (Music.GetPlaybackPosition() != 0) 
		{
			SongFinished = false;
			CurrentBPM = (int)(Music.GetPlaybackPosition() / (60f/BPM));
			CurrentSTEP = (int)(Music.GetPlaybackPosition() / (30f/BPM));
			CurrentBAR = (int)(Music.GetPlaybackPosition() / (240f/BPM));

			if (_oldSTEP != CurrentSTEP)
			{
				EmitSignal(SignalName.MusicStep, CurrentSTEP);
				_oldSTEP = CurrentSTEP;
			}

			if (_oldBPM != CurrentBPM)
			{
				EmitSignal(SignalName.MusicBeat, CurrentBPM);
				_oldBPM = CurrentBPM;
			}

			if (_oldBAR != CurrentBAR)
			{
				EmitSignal(SignalName.MusicBar, CurrentBPM);
				_oldBAR = CurrentBAR;
			}
		}
		else
		{
			SongFinished = true;
		}
	}
}
