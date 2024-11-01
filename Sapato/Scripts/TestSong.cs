using Godot;
using System;

public partial class TestSong : Node2D
{
    public override void _Ready()
    {
        DisplayServer.WindowSetSize(new Vector2I(960, 720));
        GetTree().Root.ContentScaleSize = new Vector2I(960, 720);
        DisplayServer.WindowSetPosition
        (
            new Vector2I
            (
                (DisplayServer.ScreenGetSize().X/2) - (DisplayServer.WindowGetSize().X / 2),
                (DisplayServer.ScreenGetSize().Y/2) - (DisplayServer.WindowGetSize().Y / 2)
            )
        );
    }
}
