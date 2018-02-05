using System.Linq;
using NewWave.Library.Pitches;
using NewWave.Midi;

namespace NewWave.Library.Tunings
{
	public static class GuitarTuningLibrary
	{
		public static GuitarTuning StandardGuitarTuning => new GuitarTuning(MidiPitch.E2, MidiPitch.A2, MidiPitch.D3, MidiPitch.G3, MidiPitch.B3, MidiPitch.E4);
		public static GuitarTuning StandardSevenStringGuitarTuning => new GuitarTuning(MidiPitch.B1, MidiPitch.E2, MidiPitch.A2, MidiPitch.D3, MidiPitch.G3, MidiPitch.B3, MidiPitch.E4);
		public static GuitarTuning DropDGuitarTuning => StandardGuitarTuning.Drop();

		public static GuitarTuning StandardBassTuning => StandardGuitarTuning.ToBassTuning();
		public static GuitarTuning StandardFiveStringBassTuning => StandardSevenStringGuitarTuning.ToBassTuning();

		public static GuitarTuning FromPitch(Pitch pitch, bool isDrop = false)
		{
			var t = StandardGuitarTuning;
			if (isDrop) t.Drop();

			while (t.Pitches[0].FromMidiPitch() != pitch)
			{
				t.Retune(-1);
			}

			return t;
		}

		public static GuitarTuning ToBassTuning(this GuitarTuning t)
		{
			return new GuitarTuning(t.Pitches.Take(4).Select(p => p - 12).ToArray());
		}
	}
}
