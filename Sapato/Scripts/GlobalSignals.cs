using Godot;
using System;

public partial class GlobalSignals : Node
{
	//Note
	[Signal] public delegate void PlayerNoteHitEventHandler(NoteHitEventArgs eventArgs);
	[Signal] public delegate void CPUNoteHitEventHandler(NoteHitEventArgs eventArgs);
	[Signal] public delegate void NoteHitEventHandler(NoteHitEventArgs eventArgs);

	//Music
	[Signal] public delegate void OnMusicBeatEventHandler(int Beat);
	[Signal] public delegate void OnMusicStepEventHandler(int Step);
	[Signal] public delegate void OnMusicSectionEventHandler(int Bar);

    public override void _Ready()
    {
        GlobalVariables.Signals = this;
    }
}
