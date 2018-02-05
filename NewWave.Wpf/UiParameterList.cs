using System;
using NewWave.Core;
using NewWave.Generator.Parameters;
using NewWave.Library.Chords;
using NewWave.Library.Pitches;
using NewWave.Library.Tunings;

namespace NewWave.Wpf
{
	[Serializable]
	public class UiParameterList : IParameterList
	{
		internal string Key;
		internal string Tuning;
		internal double TempoMean;
		internal double TempoStdDev;
		internal double LengthMean;
		internal double LengthStdDev;
		internal double MajorAffinity;
		internal double MinorAffinity;
		internal double DiminishedAffinity;

		internal ParameterList ToParameterList()
		{
			var key = PitchExtensions.FromString(Key);
			var isDropTuning = Tuning == "Drop";
			var guitarTuning = GuitarTuningLibrary.FromPitch(key, isDropTuning);

			return new ParameterList
			{
				MinorKeyFunc = () => key,
				GuitarTuning = guitarTuning,
				BassTuning = guitarTuning.ToBassTuning(),
				TempoMean = TempoMean,
				TempoStandardDeviation = TempoStdDev,
				LengthInSecondsMean = LengthMean,
				LengthInSecondsStandardDeviation = LengthStdDev,
				ChordProgressionFilter = node =>
				{
					switch (node.Data.Quality)
					{
						case ChordQuality.Minor:
							return node.Multiply(MajorAffinity);
						case ChordQuality.Major:
							return node.Multiply(MinorAffinity);
						case ChordQuality.Diminished:
							return node.Multiply(DiminishedAffinity);
					}
					return node;
				}
			};
		}
	}
}
