using System;
using System.Collections.Generic;
using System.Linq;
using NewWave.Library.Pitches;
using NewWave.Midi;

namespace NewWave.Library.Scales
{
	public static class ScaleLibrary
	{
		public static IEnumerable<Pitch> GetScale(Pitch root, ScaleType type)
		{
			return GetIntervals(type).Select(i => (Pitch)((int)(root + i) % 12));
		}

		private static IEnumerable<int> GetIntervals(ScaleType type)
		{
			switch (type)
			{
				case ScaleType.Major:
					return MajorScale;
				case ScaleType.Minor:
					return MinorScale;
				case ScaleType.MajorPentatonic:
					return MajorPentatonicScale;
				case ScaleType.MinorPentatonic:
					return MinorPentatonicScale;
				default:
					throw new ArgumentOutOfRangeException(nameof(type), type, null);
			}
		}

		public static MidiPitch Step(Pitch root, ScaleType scaleType, MidiPitch start, int steps)
		{
			if (steps == 0)
			{
				return start;
			}

			var scale = GetScale(root, scaleType).ToList();
			var pitchScale = scale.Select(s => s.ToMidiPitch(start.OctaveOf())).ToList();
			var octaves = Math.Abs(steps) / scale.Count + 2;
			for (var i = 1; i <= octaves; i++)
			{
				pitchScale.AddRange(scale.Select(s => s.ToMidiPitch(start.OctaveOf() + (steps > 0 ? i : -i))).ToList());
			}
			pitchScale = pitchScale.OrderBy(p => p).ToList();

			var startIndex = pitchScale.IndexOf(start);
			var returnIndex = startIndex + steps;
			return pitchScale[returnIndex];
		}

		private static IEnumerable<int> MajorPentatonicScale => new[] { 0, 2, 4, 7, 9 };
		private static IEnumerable<int> MinorPentatonicScale => new[] { 0, 3, 5, 7, 10 };
		private static IEnumerable<int> MajorScale => new[] { 0, 2, 4, 5, 7, 9, 11 };
		private static IEnumerable<int> MinorScale => new[] { 0, 2, 3, 5, 7, 8, 10 };
	}
}
