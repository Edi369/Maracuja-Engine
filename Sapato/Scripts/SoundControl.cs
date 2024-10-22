#nullable disable

using Godot;
using System;

public partial class SoundControl : Node2D
{
	private Sprite2D BGVolume;
	private ProgressBar Bar;
	private AudioStreamPlayer Audio;

	private bool IsShowing = false;
	private double Time = 0;

	public override void _Ready()
	{
		AudioController.Volume = AudioController.Volume;
		Bar = GetNode<ProgressBar>("BarVolume");
		BGVolume = GetNode<Sprite2D>("BGVolume");
		Audio = GetNode<AudioStreamPlayer>("Volume");
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("VOLUMEUP_KEY"))
		{
			Audio.Play();
			AudioController.Volume += 10;
			Time = 3;
		}
		if (Input.IsActionJustPressed("VOLUMEDOWN_KEY"))
		{
			Audio.Play();
			AudioController.Volume -= 10;
			Time = 3;
		}

		if (Time > 0)
		{
			Time -= delta;
			IsShowing = true;
		}
		else
		{
			IsShowing = false;
		}

		Bar.Value = AudioController.Volume;
		
		if (IsShowing)
		{
			this.Position = new Vector2(0, Mathf.Lerp(this.Position.Y, -16, 5f/(float)(1/delta)));
		}
		else
		{
			this.Position = new Vector2(0, Mathf.Lerp(this.Position.Y, -88, 5f/(float)(1/delta)));
		}
	}
}
