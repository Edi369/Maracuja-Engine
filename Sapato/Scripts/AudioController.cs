#nullable disable

using System;
using Godot;

public partial class AudioController : Node
{
	private static float _volume = 0;
	public static float Volume
	{ 
		get
		{
			return _volume;
		}
		set 
		{
			_volume = Math.Clamp(value, 0, 100);
			int MasterBus = AudioServer.GetBusIndex("Master");
			AudioServer.SetBusVolumeDb(MasterBus, VolumeDB);
		}
	}
	public static float VolumeDB
	{ 
		get 
		{
			if (Volume != 0) return (Volume/10 - 7) * 2;
			return -255;
		} 
	}
}
