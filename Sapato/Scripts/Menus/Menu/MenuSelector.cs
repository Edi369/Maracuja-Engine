#nullable disable

using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class MenuSelector : Node2D
{
	private bool IsSelected = false;

	public static int OptionSelected = 1;
	private float BaseYPosition = 0;
	private float YPosition = 0;

	private Sprite2D BG;
	private float BGBaseYPositon = 0;
	private float BGYPosition = 0;
	private Color BGFlickerColor = new Color(1, .035f, .572f);
	private Color BGColorBase;

	private AnimatedSprite2D StoryMode;
	private AnimatedSprite2D FreePlay;
	private AnimatedSprite2D Options;

	private List<AnimatedSprite2D> OptionsList = new List<AnimatedSprite2D>();

	public override void _Ready()
	{
		BG = this.GetParent().GetNode<Sprite2D>("BG");

		StoryMode = GetNode<AnimatedSprite2D>("StoryMode");
		OptionsList.Add(StoryMode);
		FreePlay = GetNode<AnimatedSprite2D>("FreePlay");
		OptionsList.Add(FreePlay);
		Options = GetNode<AnimatedSprite2D>("Options");
		OptionsList.Add(Options);

		BGBaseYPositon = BGYPosition = BG.Position.Y;
		BGColorBase = BG.Modulate;

		BaseYPosition = YPosition = this.Position.Y;

		ChangesAnimationOptions();
	}

	public override void _Process(double delta)
	{
		if (!IsSelected)
		{
			if (Input.IsActionJustPressed("ui_up"))
			{
				OptionSelected--;
				ChangesAnimationOptions();
			}

			if (Input.IsActionJustPressed("ui_down"))
			{
				OptionSelected++;
				ChangesAnimationOptions();
			}

			if (Input.IsActionJustPressed("ui_accept"))
			{
				FlickerBG();
			}
		}

		this.Position = new Vector2(this.Position.X, Mathf.Lerp(this.Position.Y, YPosition, 5f/(float)(1/delta)));
		BG.Position = new Vector2(BG.Position.X, Mathf.Lerp(BG.Position.Y, BGYPosition, 5f/(float)(1/delta)));
	}

	private void ChangesAnimationOptions()
	{
		if (OptionSelected > 3)
		{
			OptionSelected = 1;
		}
		else if (OptionSelected < 1)
		{
			OptionSelected = 3;
		}

		foreach (AnimatedSprite2D option in OptionsList)
		{
			if (option == OptionsList[OptionSelected-1])
			{
				option.Play("selected");
			}
			else
			{
				option.Play("idle");
			}
		}

		switch (OptionSelected)
		{
			case 1:
				YPosition = BaseYPosition+20;
				BGYPosition = BGBaseYPositon+5;
				break;

			case 2:
				YPosition = BaseYPosition;
				BGYPosition = BGBaseYPositon;
				break;
				
			case 3:
				YPosition = BaseYPosition-20;
				BGYPosition = BGBaseYPositon-5;
				break;

			default:
				YPosition = BaseYPosition;
				BGYPosition = BGBaseYPositon;
				break;
		}
	}

	private async void FlickerBG()
	{
		IsSelected = true;

		foreach (AnimatedSprite2D option in OptionsList)
		{
			if (option != OptionsList[OptionSelected-1])
			{
				option.Visible = false;
			}
		}

		for (int i = 0; i < 6; i++)
		{
			if (BG.Modulate == BGFlickerColor)
			{
				BG.Modulate = BGColorBase;
			}
			else
			{
				BG.Modulate = BGFlickerColor;
			}
			await Task.Delay(100);
		}
	}
}
