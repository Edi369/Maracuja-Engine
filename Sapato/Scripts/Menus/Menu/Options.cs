#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class Options : Node2D
{
	private bool IsSelected = false;
	private int OptionSelected = 1;
	private float BaseXPosition = 0;
	private float XPosition = 0;
	private double drogas = 0;

	private Node2D? Chosed = null;

	Dictionary<AnimatedSprite2D, Vector2> uhum = new Dictionary<AnimatedSprite2D, Vector2>();

	public override void _Ready()
	{
		UpdateSelected(1);
		BaseXPosition = this.Position.X;
	}

	public override void _Process(double delta)
	{
		joking(delta);
		if (!IsSelected)
		{
			if (Input.IsActionJustPressed("ui_right"))
			{
				OptionSelected--;

				if (OptionSelected < 1)
				{
					OptionSelected = 3;
				}

				XPosition -= Math.Abs(GetChild<Node2D>(OptionSelected-1).Position.X - GetChild<Node2D>(OptionSelected+(OptionSelected == 3 ? -2 : 0)).Position.X);
			}

			if (Input.IsActionJustPressed("ui_left"))
			{
				OptionSelected++;

				if (OptionSelected > 3)
				{
					OptionSelected = 1;
				}

				XPosition += Math.Abs(GetChild<Node2D>(OptionSelected-1).Position.X - GetChild<Node2D>(OptionSelected+(OptionSelected == 3 ? -2 : 0)).Position.X);
			}

			if (Input.IsActionJustPressed("ui_accept"))
			{
				//FlickerBG();
			}
		}
		
		GetParent<Parallax2D>().ScrollOffset = new Vector2(Mathf.Lerp(GetParent<Parallax2D>().ScrollOffset.X, XPosition, 10f/(float)(1/delta)), 0);
		UpdateSelected(delta);
	}

	private void joking(double delta)
	{
		drogas += delta;
		int _index = 0;
		foreach(Node nutch in GetNode<AlphabeticScript>("../../Gameplay").GetChildren())
		{
			_index++;
			if (nutch is AnimatedSprite2D letter)
			{	
				if (!uhum.ContainsKey(letter))
				{
					uhum.Add(letter, new Vector2(letter.Position.X, letter.Position.Y));
				}

				letter.Position = new Vector2(uhum[letter].X+((float)Mathf.Sin(drogas+_index)*4), uhum[letter].Y+((float)Mathf.Cos(drogas+_index)*4));
			}
		}
	}

	private void UpdateSelected(double delta)
	{
		int _index = 0;
		foreach (Node2D option in this.GetChildren())
		{
			_index++;

			if (OptionSelected > 3)
			{
				OptionSelected = 1;
			}
			else if (OptionSelected < 1)
			{
				OptionSelected = 3;
			}

			if (_index == OptionSelected)
			{
				Chosed = option;
				option.Modulate = new Color(1, 1, 1, 1);
			}
			else
			{
				option.Modulate = new Color(1, 1, 1, 0.5f);
			}
		}
	}
}
