#nullable disable

using System;
using Godot;

public partial class UpdateFPSText : Node
{
	public RichTextLabel BetaTest;
	public RichTextLabel FpsText;
	private double _fpsacomulative;

	public override void _Ready()
	{
		FpsText = GetNode<RichTextLabel>("Fps");
		BetaTest = GetNode<RichTextLabel>("BetaTest");


		FpsText.Modulate = new Color(1f, 1f, 1f, .8f);
		BetaTest.Modulate = new Color(1f, 1f, 1f, .5f);
	}

	private string GetColorState(double delta)
	{
		if (Math.Round(1/(float)delta) <= ConfigVariables.FpsMax/3)
		{
			return "red";
		}
		else if (Math.Round(1/(float)delta) <= ConfigVariables.FpsMax/1.5)
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
		_fpsacomulative += delta;
		if (_fpsacomulative > 0.1)
		{
			FpsText.Text = $"[color={GetColorState(delta)}][font_size=20]{Math.Round(1/(float)delta)}[/font_size][font_size=10]FPS[/font_size][/color]\n[font_size=15]RAM: {OS.GetStaticMemoryUsage()/1000000}/{OS.GetStaticMemoryPeakUsage()/1000000}[/font_size][font_size=10]mb[/font_size]";
			_fpsacomulative = 0;
		}
	}
}
