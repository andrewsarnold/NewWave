using System;
using System.Collections.Generic;
using System.Linq;
using NewWave.Core;
using NewWave.Library.Chords;
using NewWave.Library.Pitches;
using NewWave.Library.Scales;
using NewWave.Midi;

namespace NewWave.Generator.SoloLead
{
	internal static class SoloLeadGenerator
	{
		internal static IEnumerable<Note> GetSoloLead(SongInfo songInfo, int totalLength, List<Tuple<int, Chord>> chordProgression)
		{
			var notes = new List<Note>();
			const int octave = 4;
			
			for (var index = 0; index < chordProgression.Count; index++)
			{
				var chordTuple = chordProgression[index];
				var startBeat = chordTuple.Item1;
				var endBeat = index < chordProgression.Count - 1
					? chordProgression[index + 1].Item1
					: totalLength;
				var length = endBeat - startBeat;
				var chord = chordTuple.Item2;
				var scaleType = chord.Quality == ChordQuality.Minor ? ScaleType.MinorPentatonic : ScaleType.MajorPentatonic;
				var lengths = GetNoteLengths(length, songInfo.Feel);
				var pitches = GetPitches(chord, scaleType, lengths.Count, octave);
				var thisStart = (double)startBeat;

				for (var i = 0; i < lengths.Count; i++)
				{
					if (thisStart >= endBeat) continue;
					if (thisStart + lengths[i] > endBeat)
					{
						lengths[i] = endBeat - thisStart;
					}
					notes.Add(new Note(thisStart, lengths[i], pitches[i], Velocity.Ff));
					thisStart += lengths[i];
				}
			}

			return notes;
		}

		private static List<double> GetNoteLengths(int totalLength, int feel)
		{
			var thisLength = 0.0;
			var lengths = new List<double>();
			while (thisLength < totalLength)
			{
				var seg = feel == 4
					? LengthSegments4[Randomizer.Next(LengthSegments4.Count)]
					: LengthSegments3[Randomizer.Next(LengthSegments3.Count)];
				lengths.AddRange(seg);
				thisLength += seg.Sum();
			}

			return lengths;
		}

		private static List<MidiPitch> GetPitches(Chord chord, ScaleType scaleType, int count, int octave)
		{
			var interval = 0;
			var intervals = new List<int>();
			for (var i = 0; i < count; i++)
			{
				interval = Randomizer.Clamp(interval + Randomizer.Clamp(Randomizer.NextNormalized(0, 1.75), -4, 4), -7, 7);
				intervals.Add(interval);
			}

			return intervals.Select(i => ScaleLibrary.Step(chord.BasePitch, scaleType, chord.BasePitch.ToMidiPitch(octave), i)).ToList();
		}

		private static List<List<double>> LengthSegments4 => new List<List<double>>
		{
			new List<double> { 2 },
			new List<double> { 2, 1, 1 },
			new List<double> { 1, 2, 1 },
			new List<double> { 1 },
			new List<double> { 1, 1 },
			new List<double> { 0.5, 0.5 },
			new List<double> { 1.5, 1, 1.5 },
			new List<double> { 1.5, 1.5 },
			new List<double> { 0.5, 1, 0.5 },
			new List<double> { 0.5, 0.5 },
			new List<double> { 0.5, 0.5, 0.5, 0.5 },
			new List<double> { 0.5, 0.5, 1 }
		};

		private static List<List<double>> LengthSegments3 => new List<List<double>>
		{
			new List<double> { 2 },
			new List<double> { 2, 1, 1 },
			new List<double> { 1, 2, 1 },
			new List<double> { 1 },
			new List<double> { 1, 1 },
			new List<double> { 0.33, 0.33, 0.33 },
			new List<double> { 0.33, 0.67 },
			new List<double> { 0.67, 0.33 },
			new List<double> { 0.67, 0.67, 0.67 }
		};
	}
}
