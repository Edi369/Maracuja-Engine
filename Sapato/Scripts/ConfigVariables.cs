#nullable disable

using Godot;
using System;

public partial class ConfigVariables : Node
{
	public static bool AutoPause = false;

	public static int _fpxmax = 500;
	/// <summary>
	/// The Max Fps Conrigurable
	/// </summary>
	/// <value>int</value>
	public static int FpsMax
	{
		get
		{
			return _fpxmax;
		}
		set
		{
			Engine.MaxFps = value;
			_fpxmax = value;
		}
	}
}
