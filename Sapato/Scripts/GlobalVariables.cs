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

[GlobalClass]
public partial class GlobalVariables : Node
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

	public override void _Ready()
	{

	}

	public override void _Process(double delta)
	{

	}
}
