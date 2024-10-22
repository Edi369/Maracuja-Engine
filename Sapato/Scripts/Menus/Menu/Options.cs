#nullable enable

using Godot;
using System;
using System.Linq;

public partial class Options : Node2D
{
	private bool IsSelected = false;
	private int OptionSelected = 1;
	private float BaseYPosition = 0;
	private float YPosition = 0;

	private Node2D? Chosed = null;

	public override void _Ready()
	{
		UpdateSelected(1);
		BaseYPosition = this.Position.Y;
	}

	public override void _Process(double delta)
	{
		if (!IsSelected)
		{
			if (Input.IsActionJustPressed("ui_up"))
			{
				OptionSelected--;
			}

			if (Input.IsActionJustPressed("ui_down"))
			{
				OptionSelected++;
			}

			if (Input.IsActionJustPressed("ui_accept"))
			{
				//FlickerBG();
			}
		}
		
		this.Position = new Vector2(Position.X, Mathf.Lerp(this.Position.Y, YPosition, 10f/(float)(1/delta)));
		UpdateSelected(delta);
	}

	private void UpdateSelected(double delta)
	{
		switch (OptionSelected)
		{
			case 1:
				YPosition = BaseYPosition+140;
				break;
			case 2:
				YPosition = BaseYPosition+70;
				break;
			case 3:
				YPosition = BaseYPosition;
				break;
			case 4:
				YPosition = BaseYPosition-70;
				break;
		}

		int _index = 0;
		foreach (Node2D option in this.GetChildren())
		{
			if (option.Name == "SelectOptionText")
			{
				if (Chosed != null)
				{
					option.Position = new Vector2(Chosed.Position.X-60, Chosed.Position.Y);
					option.Scale = Chosed.Scale;
				}
				continue;
			}

			_index++;

			if (_index == OptionSelected)
			{
				Chosed = option;
				option.Scale = new Vector2(Mathf.Lerp(option.Scale.X, 1.2f, 10f/(float)(1/delta)), Mathf.Lerp(option.Scale.X, 1.2f, 10f/(float)(1/delta)));
			}
			else
			{
				float yes;
				if (((_index-OptionSelected)/1.15f) < 0)
				{
					yes = (OptionSelected-_index)/1.15f;
				}
				else
				{
					yes = (_index-OptionSelected)/1.15f;
				}
				option.Scale = new Vector2(Mathf.Lerp(option.Scale.X, .9f/yes, 10f/(float)(1/delta)), Mathf.Lerp(option.Scale.X, .9f/yes, 10f/(float)(1/delta)));
			}
		}
	}
}
