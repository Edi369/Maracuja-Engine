#nullable disable

using System;
using Godot;

public partial class EngineController : Node
{
	public static bool IsInFocus = true;

	private PackedScene lostFocusScene;
	private Node lostFocusIntance;
	private AnimatedSprite2D zzzLost;
	private Label TextLost;
	private Sprite2D BGLost;

    public override void _Ready()
    {
		lostFocusScene = (PackedScene)ResourceLoader.Load("res://Sapato/Objects/Extra/lost_focus.tscn");
		ResetLostFocus();

        Engine.MaxFps = ConfigVariables.FpsMax;
    }

    public override void _Notification(int what)
	{
	    if (what == MainLoop.NotificationApplicationFocusIn)
	    {
			OS.LowProcessorUsageMode = false;
	        Engine.MaxFps = ConfigVariables.FpsMax;
			CallDeferred("remove_child", lostFocusIntance);
			lostFocusIntance.QueueFree();
			ResetLostFocus();
			
			IsInFocus = true;

			if (ConfigVariables.AutoPause)
			{
				GetTree().Paused = false;
			}
	    }
	    else if (what == MainLoop.NotificationApplicationFocusOut)
	    {
			OS.LowProcessorUsageMode = true;
	        Engine.MaxFps = 30;
			IsInFocus = false;

			zzzLost.Play();
			CallDeferred("add_child", lostFocusIntance);

			if (ConfigVariables.AutoPause)
			{
				GetTree().Paused = true;
			}
	    }
	}

	public void ResetLostFocus()
	{
		lostFocusIntance = lostFocusScene.Instantiate();
		zzzLost = lostFocusIntance.GetNode<AnimatedSprite2D>("ZZZ");
		TextLost = lostFocusIntance.GetNode<Label>("Text");
		BGLost = lostFocusIntance.GetNode<Sprite2D>("BG");
	}

    public override void _Process(double delta)
    {
		if(!IsInFocus)
		{
			BGLost.Modulate = new Color(1, 1, 1, Mathf.Lerp(BGLost.Modulate.A, .498f, 5f/(float)(1/delta)));

			zzzLost.Modulate = new Color(1, 1, 1, Mathf.Lerp(zzzLost.Modulate.A, 1, 5f/(float)(1/delta)));
			zzzLost.Position = new Vector2(418, Mathf.Lerp(zzzLost.Position.Y, 358, 5f/(float)(1/delta)));

			TextLost.Modulate = new Color(1, 1, 1, Mathf.Lerp(TextLost.Modulate.A, 1, 5f/(float)(1/delta)));
			TextLost.Position = new Vector2(579, Mathf.Lerp(TextLost.Position.Y, 323, 5f/(float)(1/delta)));
		}

		if (Input.IsActionJustPressed("FULLSCREEN_KEY"))
		{
			if (DisplayServer.WindowGetMode() == DisplayServer.WindowMode.ExclusiveFullscreen)
			{
				DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
			}
			else
			{
				DisplayServer.WindowSetMode(DisplayServer.WindowMode.ExclusiveFullscreen);
			}
		}
    }
}
