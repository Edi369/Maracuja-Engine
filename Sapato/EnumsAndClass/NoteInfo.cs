using Godot;
using Newtonsoft.Json;

public class NoteInfo
{
	[JsonIgnore]
	public AnimatedSprite2D Note = null;
	[JsonIgnore]
	public Vector2 Position = new Vector2();
	public float TimeNote = 0f;
	public string Comment = "";
	public int Direction = 0;
	public string StrumLine = "Dad";
	public float? LongNoteLenght = null;
	public int NoteType = 0;
	public bool IsPlayAnimation = true;
	[JsonIgnore]
	public int NoteId = 0;
}
