using Godot;

public partial class NoteHitEventArgs : Node
{
	public NoteInfo Note { get; set; }
	public string Rating { get; set; }
	public int Score { get; set; }
	public double Delay { get; set; }
	public bool IsSusteinNote { get; set; }
}