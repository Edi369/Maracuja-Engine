#nullable disable

using Godot;
using System;
using System.Linq;

[Tool]
[GlobalClass]
public partial class AlphabeticScript : Node2D
{
	[Export]
	public string FnfText { get; set; } = String.Empty;
	
	public AnimatedSprite2D DummyLetter;

	public override void _Ready()
	{
		if (!Engine.IsEditorHint())
		{
			if (FnfText != "" || FnfText != String.Empty)
			{
				GenerateFnfText(FnfText);
			}
		}
	}

	public override void _Process(double delta)
	{
		if (Engine.IsEditorHint())
		{
			if (FnfText != "" || FnfText != String.Empty)
			{
				GenerateFnfText(FnfText);
			}
		}
	}

	public void GenerateFnfText(string Text)
	{
		DummyLetter = GetNode<AnimatedSprite2D>("DummyLetter");
		DummyLetter.Visible = false;
		foreach (AnimatedSprite2D sprite in this.GetChildren())
		{
			if (sprite != DummyLetter)
			{
				sprite.QueueFree();
				this.RemoveChild(sprite);
			}
		}
		string[] textCool = Text.Split("\\n");
		int _indexChar = 0;
		float YPosition = 0;
		foreach (string text in textCool)
		{
			foreach (char Char in text)
			{
				_indexChar++;
				if (Char.ToString() == " ")
				{
					continue;
				}
				AnimatedSprite2D letter = new AnimatedSprite2D();
				letter.SpriteFrames = DummyLetter.SpriteFrames;
				letter.Name = _indexChar.ToString() + Char.ToString();
				letter.Play(Char.ToUpper(Char).ToString());
				letter.Position = new Vector2(50*(_indexChar-1), YPosition);
				this.AddChild(letter);
			}
			YPosition += 70;
			_indexChar = 0;
		}
	}
}
