using Godot;
using NetDiscordRpc;
using NetDiscordRpc.Core.Logger;
using NetDiscordRpc.RPC;
using Button = NetDiscordRpc.RPC.Button;

public partial class DiscordRpc : Node
{
	public static DiscordRPC discordrpc;
    public static RichPresence DiscordRichBase;

	public override void _Ready()
	{
		discordrpc = new DiscordRPC("1299867652512153671");
		discordrpc.Logger = new ConsoleLogger();
		discordrpc.Initialize();

        DiscordRichBase = new RichPresence()
        {
            //State = "M.E V0.5",
            Details = "Maracujing",
                            
            Assets = new Assets()
            {
                LargeImageKey = "maracujaengineappicon",
                LargeImageText = "Maracuja Engine V0.5 (Beta)",
                //SmallImageKey = "SMALL_IMAGE_KEY_HERE",
                //SmallImageText = "SMALL_IMAGE_TEXT_HERE"
            },
                    
            Timestamps = Timestamps.Now,
                    
            Buttons = new Button[]
            {
                new() { Label = "Cool Button", Url = "https://youtu.be/8f434o5Btbs?si=wzxtHEulmDXu5ZCC" }
            }
        };

		discordrpc.SetPresence(DiscordRichBase);

        discordrpc.Invoke();
	}

    public static void UpdateDetails(string text)
    {
        DiscordRichBase.Details = text;
        discordrpc.SetPresence(DiscordRichBase);
    }

	//public override void _Process(double delta)
	//{
	//}
}
