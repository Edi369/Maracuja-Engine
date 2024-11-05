using System.Xml.Serialization;

[XmlRoot("Character")]
public class Character
{
    [XmlAttribute("name")]
    public string Name { get; set; } = "maracuja";
    [XmlAttribute("path")]
    public string Path { get; set; } = "res://Sapato/Images/Characters";
    [XmlAttribute("iconame")]
    public string IcoName { get; set; } = "maracuja";
    [XmlAttribute("color")]
    public string Color { get; set; } = "#FCFF4F";
    [XmlElement("Anim")]
    public Anim[] Anims;
}

public class Anim
{
    [XmlAttribute("xpos")]
    public int XPos { get; set; } = 0;
    [XmlAttribute("ypos")]
    public int YPos { get; set; } = 0;
    [XmlAttribute("xlmname")]
    public string XlmName { get; set; } = "maracuja";
    [XmlAttribute("identname")]
    public string IdentName { get; set; } = "idle";
    [XmlAttribute("fps")]
    public int Fps { get; set; } = 24;
    [XmlAttribute("loop")]
    public bool Loop { get; set; } = false;
}
