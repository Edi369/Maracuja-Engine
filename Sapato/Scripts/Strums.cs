#nullable disable

using Godot;
using System;

public partial class Strums : Node2D
{
	[Export]
	public bool Player = false;
	
	public AnimatedSprite2D Up;
	public AnimatedSprite2D Down;
	public AnimatedSprite2D Left;
	public AnimatedSprite2D Right;

	public override void _Ready()
	{
		Up = GetNode<AnimatedSprite2D>("Up");
		Down = GetNode<AnimatedSprite2D>("Down");
		Left = GetNode<AnimatedSprite2D>("Left");
		Right = GetNode<AnimatedSprite2D>("Right");
	}

	//public void GhostPress()
	//{
	//	if (Input.IsActionPressed("UP_KEY")) Up.Scale = new Vector2(.9f, .9f);
	//	else Up.Scale = new Vector2(1f, 1f);
	//	if (Input.IsActionPressed("DOWN_KEY")) Down.Scale = new Vector2(.9f, .9f);
	//	else Down.Scale = new Vector2(1f, 1f);
	//	if (Input.IsActionPressed("LEFT_KEY")) Left.Scale = new Vector2(.9f, .9f);
	//	else Left.Scale = new Vector2(1f, 1f);
	//	if (Input.IsActionPressed("RIGHT_KEY")) Right.Scale = new Vector2(.9f, .9f);
	//	else Right.Scale = new Vector2(1f, 1f);
	//}
}
