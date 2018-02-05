using NewWave.Midi;

namespace NewWave.Library.Tunings
{
	public class GuitarTuning
	{
		public readonly MidiPitch[] Pitches;
		public bool IsDropTuning { get; private set; }

		public GuitarTuning(params MidiPitch[] pitches)
		{
			Pitches = pitches;
		}

		public GuitarTuning Retune(int stepsUp)
		{
			for (var i = 0; i < Pitches.Length; i++)
			{
				Pitches[i] += stepsUp;
			}
			return this;
		}

		public GuitarTuning Drop()
		{
			Pitches[0] -= 2;
			IsDropTuning = true;
			return this;
		}
	}
}
