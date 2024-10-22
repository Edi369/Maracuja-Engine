using Godot;
using System;
using System.Collections.Generic;

public class ChartInfo
{
	public string dif { get; set; }
	public string even { get; set;}
	public DateTime InitialTime { get; set; }
	public int scrollspeed { get; set; }
	public float ChartMaracujaVersion { get; set; } = 1f;
	public List<NoteInfo> notes = new List<NoteInfo>();
}
