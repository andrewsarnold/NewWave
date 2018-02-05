using System;
using System.Linq;
using NewWave.Library.Chords;
using NewWave.Library.Pitches;
using NewWave.Library.Tunings;

namespace NewWave.Generator.Parameters
{
	public static class ParameterLibrary
	{
		public static Func<MarkovChainNode<Chord>, MarkovChainNode<Chord>> MinorFilter
		{
			get
			{
				return n => n.Data.Quality != ChordQuality.Minor && n.Data.Quality != ChordQuality.Diminished
					? new MarkovChainNode<Chord>(n.Data, n.Probability * 0.25, n.ChildNodes?.Where(c => c.Probability > 0.08).ToList())
					: n;
			}
		}

		public static Func<MarkovChainNode<Chord>, MarkovChainNode<Chord>> MajorFilter
		{
			get
			{
				return n =>
					n.Data.Quality == ChordQuality.Minor
						? new MarkovChainNode<Chord>(n.Data, n.Probability * 0.25, n.ChildNodes?.Where(c => c.Probability > 0.08).ToList())
						: n;
			}
		}

		public static Pitch GetKey(GuitarTuning tuning)
		{
			var possibleKeys = new[] { tuning.Pitches[0], tuning.Pitches[0] + 2, tuning.Pitches[1] }; // e.g. E, F#, A (0, 2, 5)
			return possibleKeys[Randomizer.Next(possibleKeys.Length)].FromMidiPitch();
		}
	}
}
