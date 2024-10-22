using Godot;
using System;

public partial class MusicControl : Node
{
	public AudioStreamPlayer Music;

	[Export]
	public int BPM = 135;

	public int CurrentBPM = 0;
	private int _oldBPM = 0;
	public int CurrentSTEP = 0;
	private int _oldSTEP = 0;
	public int CurrentBAR = 0;
	private int _oldBAR = 0;

	public bool SongFinished = false;

	Label DebugText;

	[Signal]
	public delegate void MusicBeatEventHandler(int Beat);

	[Signal]
	public delegate void MusicStepEventHandler(int Step);

	[Signal]
	public delegate void MusicBarEventHandler(int Bar);

	public override void _Ready()
	{
		Music = GetNode<AudioStreamPlayer>("Inst");
		DebugText = GetNode<Label>("../Camera2D/InfoDebug");
		Music.Play();
	}

	public override void _Process(double delta)
	{
		if (!SongFinished && Music.GetPlaybackPosition() != 0) 
		{
			CurrentBPM = (int)(Music.GetPlaybackPosition() / (60f/BPM));
			CurrentSTEP = (int)(Music.GetPlaybackPosition() / (30f/BPM));
			CurrentBAR = (int)(Music.GetPlaybackPosition() / (240f/BPM));

			DebugText.Text = @$"{CurrentBPM} :BPM
{CurrentSTEP} :STEP
{CurrentBAR} :BAR";

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
