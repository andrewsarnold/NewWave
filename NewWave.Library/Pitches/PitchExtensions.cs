using System;
using NewWave.Midi;

namespace NewWave.Library.Pitches
{
	public static class PitchExtensions
	{
		public static MidiPitch AddOctave(this MidiPitch pitch, int octaves)
		{
			return pitch + 12 * octaves;
		}

		public static string NoteName(this Pitch pitch)
		{
			var toString = pitch.ToString();
			var letter = toString.Substring(0, 1);
			var isSharp = toString.Contains("Sharp");
			return string.Format("{0}{1}", letter, isSharp ? "#" : "");
		}

		public static string NoteName(this MidiPitch pitch)
		{
			var toString = pitch.ToString();
			var letter = toString.Substring(0, 1);
			var isSharp = toString.Contains("Sharp");
			return string.Format("{0}{1}", letter, isSharp ? "#" : "");
		}

		public static MidiPitch ToMidiPitch(this Pitch p, int octave)
		{
			var pitchDiff = ((int)p - (int)MidiPitch.CNeg1) % 12;
			return MidiPitch.CNeg1 + pitchDiff + 12 * (octave + 1);
		}

		public static Pitch FromMidiPitch(this MidiPitch p)
		{
			return (Pitch)((int)p % 12);
		}
		
		public static int OctaveOf(this MidiPitch p)
		{
			return (int)p / 12 - 1;
		}

		public static Pitch FromString(string input)
		{
			if (input.Length < 1 || input.Length > 2) throw new ArgumentOutOfRangeException();

			Pitch p;
			switch (input.ToUpper()[0])
			{
				case 'A':
					p = Pitch.A;
					break;
				case 'B':
					p = Pitch.B;
					break;
				case 'C':
					p = Pitch.C;
					break;
				case 'D':
					p = Pitch.D;
					break;
				case 'E':
					p = Pitch.E;
					break;
				case 'F':
					p = Pitch.F;
					break;
				default:
					p = Pitch.G;
					break;
			}

			if (input.Length == 2 && input[1] == '#')
			{
				p += 1;
				if (p > Pitch.B) p -= 12;
			}
			else if (input.Length == 2 && input[1] == 'b')
			{
				p -= 1;
				if (p < Pitch.C) p += 12;
			}

			return p;
		}
	}
}
