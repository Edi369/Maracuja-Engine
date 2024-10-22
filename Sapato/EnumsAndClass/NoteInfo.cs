using Godot;
using Newtonsoft.Json;
using System.Collections;

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
	public bool IsLongNote = false;
	public int NoteType = 0;
	public bool IsPlayAnimation = true;
	public int NoteId = 0;

	//public NoteInfo(
	//	AnimatedSprite2D note = null,
	//	Vector2 position = new Vector2(),
	//	float timeNote = 0f, string comment = "",
	//	int direction = 0, string strumLine = "Dad",
	//	bool isLongNote = false,
	//	string noteType = "Normal",
	//	bool isPlayAnimation = true)
	//{
	//	Note = note;
	//	Position = position;
	//	TimeNote = timeNote;
	//	Comment = comment;
	//	Direction = direction;
	//	StrumLine = strumLine;
	//	IsLongNote = isLongNote;
	//	NoteType = noteType;
	//	IsPlayAnimation = isPlayAnimation;
	//}
}
