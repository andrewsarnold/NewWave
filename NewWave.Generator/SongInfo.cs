using System;
using NewWave.Core;
using NewWave.Generator.Parameters;

namespace NewWave.Generator
{
	public class SongInfo
	{
		public readonly TimeSignature TimeSignature;
		public readonly int Feel;

		public int Tempo;
		public double LengthInSeconds;

		private ParameterList _parameters;
		public ParameterList Parameters
		{
			get { return _parameters; }
			set
			{
				_parameters = value;
				if (_parameters != null)
				{
					Tempo = (int)Randomizer.NextNormalized(value.TempoMean, value.TempoStandardDeviation);
					LengthInSeconds = Math.Max(Randomizer.NextNormalized(value.LengthInSecondsMean, value.LengthInSecondsStandardDeviation), 0);
				}
			}
		}

		public SongInfo(TimeSignature timeSignature, int feel)
		{
			TimeSignature = timeSignature;
			Feel = feel;
			Parameters = new ParameterList();
		}
	}
}
