using Godot;
using System;

public partial class WaveForm : Line2D
{
	[Export] float valuetest = 0f;
	public byte[] dataBufferTest;

	public override void _Ready()
	{
		GetParent<ChartEditor>().LoadMusicChart += (data) =>
		{
			if (ChartEditor.Voices.Stream != null)
			{
				return;
			}

			dataBufferTest = data;
			int _indexs = -1;
			short[] samples = ExtractSamplesFromWav(data, out int sampleRate);
			
			float bpm = GetNode<ChartMusicControl>("../MusicControl").BPM;
		    float songLengthInSeconds = samples.Length / (float)sampleRate;

		    float samplesPerBeat = sampleRate * 60f / bpm;
		    float yScalingFactor = 1000f / songLengthInSeconds;

			foreach (short dataSong in samples)
			{
				float songData = (dataSong)/300;
				_indexs++;
				float amplitude = dataSong / 32768f * 150;
				float yPosition = _indexs / samplesPerBeat * yScalingFactor;

				if(_indexs % 300 > 0)
				{
					continue;
				}

				//float YPos = _indexs*(((float)ChartEditor.Music.Stream.GetLength()/data.Length)/GetNode<ChartMusicControl>("../MusicControl").BPM);
				float YPos = _indexs/samplesPerBeat;

				this.AddPoint(new Vector2(-songData, yPosition*valuetest));
				this.AddPoint(new Vector2(songData, yPosition*valuetest));
			}
		};

		GetParent<ChartEditor>().LoadVoicesChart += (data) =>
		{
			dataBufferTest = data;
			int _indexs = -1;
			short[] samples = ExtractSamplesFromWav(data, out int sampleRate);
			
			float bpm = GetNode<ChartMusicControl>("../MusicControl").BPM;
		    float songLengthInSeconds = samples.Length / (float)sampleRate;

		    float samplesPerBeat = sampleRate * 60f / bpm;
		    float yScalingFactor = 1000f / songLengthInSeconds;

			foreach (short dataSong in samples)
			{
				float songData = (dataSong)/300;
				_indexs++;
				float amplitude = dataSong / 32768f * 150;
				float yPosition = _indexs / samplesPerBeat * yScalingFactor;

				if(_indexs % 300 > 0)
				{
					continue;
				}

				//float YPos = _indexs*(((float)ChartEditor.Music.Stream.GetLength()/data.Length)/GetNode<ChartMusicControl>("../MusicControl").BPM);
				float YPos = _indexs/samplesPerBeat;

				this.AddPoint(new Vector2(-songData, yPosition*valuetest));
				this.AddPoint(new Vector2(songData, yPosition*valuetest));
			}
		};
	}

	public override void _Process(double delta)
	{
		this.Position = new Vector2(this.Position.X, GetParent<ChartEditor>().FakeYPosition);

		if (Input.IsActionJustPressed("ui_accept"))
		{
			this.Points = Array.Empty<Vector2>();
			GetParent<ChartEditor>().EmitSignal("LoadVoicesChart", dataBufferTest);
		}
	}

	public static short[] ExtractSamplesFromWav(byte[] wavData, out int sampleRate)
    {
		sampleRate = BitConverter.ToInt32(wavData, 24);
        int bitsPerSample = BitConverter.ToInt16(wavData, 34);
		int dataStartIndex = Array.IndexOf(wavData, (byte)'d', 36) + 8;

        int bytesPerSample = bitsPerSample / 8;
        int sampleCount = (wavData.Length - dataStartIndex) / bytesPerSample;

        short[] samples = new short[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            samples[i] = BitConverter.ToInt16(wavData, dataStartIndex + i * bytesPerSample);
        }

        return samples;
    }
}
