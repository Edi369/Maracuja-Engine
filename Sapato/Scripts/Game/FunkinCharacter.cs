using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

[GlobalClass]
public partial class FunkinCharacter : AnimatedSprite2D
{
	public string AltAnimPrefix = "";
	[Export] public string Strum = "";
	public bool IsInAnimation = false;
	public double TimeOffset = 0;
	public Character characterinfo;
	public Dictionary<string, string> GeneralAnimations = new Dictionary<string, string>();

	public override void _Ready()
	{
		GlobalVariables.Signals.NoteHit += PlayAnimOnNote;
		GlobalVariables.Signals.NoteMiss += PlayAnimOnMiss;
		GlobalVariables.Signals.OnMusicBeat += PlayAnimIdle;

		LoadDataCharacter();

		if(Strum == "")
		{
			Strum = Name;
		}
	}

	public void LoadDataCharacter()
	{
		string finalPath = ProjectSettings.GlobalizePath($"{FilesController.Characters.Path}/{Name}.xml");

		var xmlSerializer = new XmlSerializer(typeof(Character));
		using var fileXml = new FileStream(finalPath, FileMode.Open);
		characterinfo = (Character)xmlSerializer.Deserialize(fileXml);

		SpriteFrames = XmlAtlasImporter.LoadFromXml($"{characterinfo.Path}.png", $"{characterinfo.Path}.xml");

		foreach(Anim anim in characterinfo.Anims)
		{
			GeneralAnimations.Add(anim.IdentName, anim.XlmName);
			SpriteFrames.SetAnimationLoop(anim.XlmName, anim.Loop);
			SpriteFrames.SetAnimationSpeed(anim.XlmName, anim.Fps);
		}	
	}

	public void PlayAnimIdle(int o)
	{
		if (o % 2 != 0 || IsInAnimation)
		{
			return;
		}

		PlayAnimation("idle", true, false);
	}

	public void PlayAnimOnNote(NoteHitEventArgs o)
	{
		if(o.Note.StrumLine != Strum)
		{
			return;
		}

		PlayAnimation(GetAnimationIndex(o.Note.Direction));
	}

		public void PlayAnimOnMiss(NoteHitEventArgs o)
	{
		if(o.Note.StrumLine != Strum)
		{
			return;
		}

		PlayAnimation($"{GetAnimationIndex(o.Note.Direction)}-miss");
	}

	public string GetAnimationIndex(int index)
	{
		switch (index % 4)
		{
			default:
				return "left";
			case 1:
				return "down";
			case 2:
				return "up";
			case 3:
				return "right";
		}
	}

	public void PlayAnimation(string animation, bool usePrefix = true, bool resetTime = true)
	{
		if (resetTime)
		{
			IsInAnimation = true;
			TimeOffset = 0;
		}

		if (!usePrefix)
		{
			if (GeneralAnimations.ContainsKey(animation))
			{
				Play(GeneralAnimations[animation]);
			}
			else
			{
				GD.PrintErr($"Animation dont found! {animation}");
			}
			return;
		}

		if (GeneralAnimations.ContainsKey(animation))
		{
			Play(GeneralAnimations[animation] + AltAnimPrefix);
		}
		else
		{
			GD.PrintErr($"Animation dont found! {animation}");
		}
	}

	public override void _Process(double delta)
	{
		TimeOffset += delta;

		if (TimeOffset >= 60f/GlobalVariables.BPM)
		{
			IsInAnimation = false;
		}
	}

	public override void _ExitTree()
	{
		GlobalVariables.Signals.NoteHit -= PlayAnimOnNote;
		GlobalVariables.Signals.OnMusicBeat -= PlayAnimIdle;
	}
}
