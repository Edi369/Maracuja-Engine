#nullable disable

using System;
using Godot;

public partial class UpdateFPSText : Node
{
	public RichTextLabel BetaTest;
	public RichTextLabel FpsText;

	public override void _Ready()
	{
		FpsText = GetNode<RichTextLabel>("Fps");
		BetaTest = GetNode<RichTextLabel>("BetaTest");


		FpsText.Modulate = new Color(1f, 1f, 1f, .8f);
		BetaTest.Modulate = new Color(1f, 1f, 1f, .5f);
	}

	private string GetColorState()
	{
		if (Engine.GetFramesPerSecond() <= ConfigVariables.FpsMax/3)
		{
			return "red";
		}
		else if (Engine.GetFramesPerSecond() <= ConfigVariables.FpsMax/1.5)
		{
			return "yellow";
		}
		else
		{
			return "white";
		}
	}

	public override void _Process(double delta)
	{
		FpsText.Text = $"[color={GetColorState()}][font_size=20]{Engine.GetFramesPerSecond()}[/font_size][font_size=10]FPS[/font_size][/color]\n[font_size=15]RAM: {OS.GetStaticMemoryUsage()/1000000}/{OS.GetStaticMemoryPeakUsage()/1000000}[/font_size][font_size=10]mb[/font_size]";
	}
}
