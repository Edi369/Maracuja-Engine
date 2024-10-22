#nullable disable

using Godot;
using System;

public partial class UpdateInfoText : RichTextLabel
{
	public override void _Process(double delta)
	{
		this.Text = $"[center]Scory: {GlobalVariables.Score} ・ Accuracy: [F-](00.00%) ・ Misses: {GlobalVariables.Misses}[/center]";
	}
}
