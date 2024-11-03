#nullable disable

using System;
using System.Collections.Generic;
using Godot;

public class Strum
{
	public AnimatedSprite2D LEFT;
	public AnimatedSprite2D DOWN;
	public AnimatedSprite2D UP;
	public AnimatedSprite2D RIGHT;
	public Strums STRUM;

	public Strum(AnimatedSprite2D left = null, AnimatedSprite2D down = null, AnimatedSprite2D up = null, AnimatedSprite2D right = null)
	{
		LEFT = left;
		DOWN = down;
		UP = up;
		RIGHT = right;
	}
}

public class GlobalVariables
{
	public static Dictionary<string, Strum> StrumLines = new Dictionary<string, Strum>();

	/// <summary>
	/// The Max Health Clamp
	/// </summary>
	/// <value>float</value>
	public static float HealthMax {get; set;} = 2;

	/// <summary>
	/// The Min Health Clamp
	/// </summary>
	/// <value>float</value>
	public static float HealthMin {get; set;} = 0;

	private static float _health = 1;
	/// <summary>
	/// The Health
	/// </summary>
	/// <value>float</value>
	public static float Health
	{ 
		get { return _health; }
		set
		{
			_health = Math.Clamp(value, HealthMin, HealthMax);
		}
	}

	/// <summary>
	/// The Score of The Player
	/// </summary>
	/// <value>int</value>
	public static int Score { get; set; } = 0;

	/// <summary>
	/// The Misses of the Player
	/// </summary>
	/// <value>int</value>
	public static int Misses { get; set;} = 0;

	public static string CurrentSong { get; set; } = "Timeless Daydream";
	public static string CurrentDifficult { get; set; } = "sata andagui";

	public static int CameraShakeBeats { get; set; } = 4;
	public static double CameraGAMEBaseZoom { get; set; } = 1;
	public static double HUDBaseZoom { get; set; } = 1;
	public static double ScrollSpeed { get; set; } = 1;
	public static int BPM { get; set; } = 1;

	public static CanvasLayer HUD { get; set; }
	public static Camera2D CamGame { get; set; }
	public static GlobalSignals Signals { get; set; }
}
