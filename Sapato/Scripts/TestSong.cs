using Godot;
using System;

public partial class TestSong : Node2D
{
    Parallax2D freakyBg;

    public override void _Ready()
    {
        freakyBg = GetNode<Parallax2D>("freakyBg");
        GlobalVariables.Engine.ChangeResolutionGame(1295, 971);

        GetNode<ChartMusicControl>("HUD/Strum/NoteController/MusicControl").MusicBeat += (o) =>
        {
            switch (o)
            {
                case 128:
                Tween tween = CreateTween();
                GlobalVariables.CameraShakeBeats = 1;
                tween.TweenMethod(Callable.From<double>((o) => {GlobalVariables.ScrollSpeed = o;}), GlobalVariables.ScrollSpeed, 1.3f, 1.0f);
                //tween.TweenProperty(GetNode<NoteController>("HUD/Strum/NoteController"), "ScrollSpeed", 1.3, 1.0f);
                break;
                
                case 256:
                GlobalVariables.CameraShakeBeats = 4;
                break;

                case 326:
                GlobalVariables.CameraShakeBeats = 1;
                break;
                
                case 454:
                GlobalVariables.CameraShakeBeats = 4;
                break;
            }
        };
    }

    public override void _Process(double delta)
    {
        freakyBg.ScrollOffset = new Vector2(freakyBg.ScrollOffset.X+50f/(float)(1/delta), freakyBg.ScrollOffset.Y+50f/(float)(1/delta));
    }
}
