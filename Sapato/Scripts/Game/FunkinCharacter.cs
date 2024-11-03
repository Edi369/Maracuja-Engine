using Godot;
using System;

public partial class FunkinCharacter : AnimatedSprite2D
{
	public string AltAnimPrefix = "";
	public bool IsInAnimation = false;
	public double TimeOffset = 0;

	public override void _Ready()
	{
		GlobalVariables.Signals.NoteHit += (o) =>
		{
			if(o.Note.StrumLine != this.Name)
			{
				return;
			}

			PlayAnimation(GetAnimationIndex(o.Note.Direction));
		};

		GlobalVariables.Signals.OnMusicBeat += (o) =>
		{
			if (o % 2 != 0 || IsInAnimation)
			{
				return;
			}

			PlayAnimation("idle", true, false);
		};
	}

	public string GetAnimationIndex(int index)
	{
		switch (index % 4)
		{
			default:
				return "left";
			case 1:
				return "down";
			case 2:
				return "up";
			case 3:
				return "right";
		}
	}

	public void PlayAnimation(string animation, bool usePrefix = false, bool resetTime = true)
	{
		if (resetTime)
		{
			IsInAnimation = true;
			TimeOffset = 0;
		}

		if (!usePrefix)
		{
			this.Play(animation);
			return;
		}

		this.Play(animation + AltAnimPrefix);
	}

	public override void _Process(double delta)
	{
		TimeOffset += delta;

		if (TimeOffset >= 60f/GlobalVariables.BPM)
		{
			IsInAnimation = false;
		}
	}
}
