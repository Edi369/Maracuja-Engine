using Godot;
using System;
using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Text.RegularExpressions;

//importing stuff :cool:
[XmlRoot("TextureAtlas")]
public class TextureAtlas
{
	[XmlAttribute("imagePath")]
	public string ImagePath { get; set; } = "";
	[XmlAttribute("width")]
	public int Width { get; set; } = 0;
	[XmlAttribute("height")]
	public int Height { get; set; } = 0;
	[XmlElement("SubTexture")]
	public SubTexture[] SubTextures;
}

public class SubTexture
{
	[XmlAttribute("name")]
	public string Name { get; set; } = "";
	[XmlAttribute("x")]
	public int X { get; set; } = 0;
	[XmlAttribute("y")]
	public int Y { get; set; } = 0;
	[XmlAttribute("width")]
	public int Width { get; set; } = 0;
	[XmlAttribute("height")]
	public int Height { get; set; } = 0;
	[XmlAttribute("frameX")]
	public int FrameX { get; set; } = 0;
	[XmlAttribute("frameY")]
	public int FrameY { get; set; } = 0;
	[XmlAttribute("frameWidth")]
	public int FrameWidth { get; set; } = 0;
	[XmlAttribute("frameHeight")]
	public int FrameHeight { get; set; } = 0;
}

public class XmlAtlasImporter
{
	//cool
	public static SpriteFrames LoadFromXml(string pathImg, string pathXml)
	{
		string finalPathImg = ProjectSettings.GlobalizePath(pathImg);
		string finalPathXml = ProjectSettings.GlobalizePath(pathXml);

		var xmlSerializer = new XmlSerializer(typeof(TextureAtlas));
		using var fileXml = new FileStream(finalPathXml, FileMode.Open);
		TextureAtlas textureAtlas = (TextureAtlas)xmlSerializer.Deserialize(fileXml);

		Dictionary<string, List<SubTexture>> animations = new Dictionary<string, List<SubTexture>>();

		foreach(SubTexture subTexture in textureAtlas.SubTextures)
		{
			List<SubTexture> animation = new List<SubTexture>();
			string animationName = Regex.Replace(subTexture.Name, @"[\d-]", string.Empty);

			if(animations.ContainsKey(animationName))
			{
				animation = animations[animationName];
				animation.Add(subTexture);
				animations[animationName] = animation;
			}
			else
			{
				animation.Add(subTexture);
				animations.Add(animationName, animation);
			}
		}

		SpriteFrames sprites = new SpriteFrames();
		AnimatedSprite2D spritetest = new AnimatedSprite2D();
		Image imgBase = new Image();

		imgBase.Load(finalPathImg);
		
		foreach(KeyValuePair<string, List<SubTexture>> animation in animations)
		{
			sprites.AddAnimation(animation.Key);

			foreach(SubTexture subTexture in animation.Value)
			{
				try
				{
					Image cropedImg = new Image();
					Image transpImg = Image.CreateEmpty(subTexture.FrameHeight, subTexture.FrameWidth, false, Image.Format.Rgba8);
					ImageTexture imageTexture = new ImageTexture();

					cropedImg = imgBase.GetRegion(new Rect2I()
					{
						Position = new Vector2I(subTexture.X, subTexture.Y),
						Size =  new Vector2I(subTexture.Width, subTexture.Height),
					});

					for(int w = 1; w < subTexture.Width; w++)
					{
						for(int h = 1; h < subTexture.Height; h++)
						{
							transpImg.SetPixel(w-subTexture.FrameX, h-subTexture.FrameY, cropedImg.GetPixel(w, h));
						}
					}

					imageTexture.SetImage(transpImg);

					bool canParse = int.TryParse(Regex.Replace(Regex.Replace(subTexture.Name, " ", string.Empty), "[A-Za-z ]", string.Empty), out var index);

					if(canParse)
					{
						sprites.AddFrame(animation.Key, imageTexture, 1, index);
					}
					else
					{
						GD.PrintErr($"Can't get name of the animation... {subTexture.Name}");
					}

					transpImg.Dispose();
					cropedImg.Dispose();
				}
				catch (Exception ex)
				{
					GD.PrintErr($"Cannot load atlas file! Ex: {ex}");
				}
			}
		}

		return sprites;
	}
}