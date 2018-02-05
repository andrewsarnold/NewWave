using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NewWave.Core;
using NewWave.Generator;
using NewWave.Generator.ChordProgressions;
using NewWave.Generator.Parameters;
using NewWave.Library.Chords;
using NewWave.Midi;

namespace NewWave.Test.GeneratorTests
{
	[TestClass]
	public class BasicGeneratorTests
	{
		private const MidiPitch LowestPitch = MidiPitch.E2;
		private static readonly ParameterList Parameters = new ParameterList
		{
			MinorKeyFunc = () => new List<MidiPitch> { LowestPitch, LowestPitch + 5, LowestPitch + 2 }[Randomizer.GetWeightedIndex(new List<double>
			{
				0.5, 0.3, 0.2
			})],
			TimeSignatureFunc = () => new TimeSignature(Randomizer.ProbabilityOfTrue(0.75) ? 4 : 3, 4),
			FeelFunc = t => Randomizer.ProbabilityOfTrue(t.BeatCount == 4 ? 0.65 : 0.8) ? 4 : 3,
			ChordProgressionFilter = MinorOrDiminshedFilter
		};

		[TestMethod]
		public void GenerateTest()
		{
			var song = new GeneratedSong();
			song.Generate(Parameters);
			Console.WriteLine(song.WriteStats());
			foreach (var section in song.Sections)
			{
				Console.WriteLine("{0}: {1} meas, {2}", section.Type, section.Measures, string.Join(" - ", section.Chords.Select(c => c.Item2)));
			}
		}

		[TestMethod]
		public void RenderTest()
		{
			var song = new GeneratedSong();
			Common.RenderAndPlay(Parameters, song, "output.mid");
			foreach (var section in song.Sections)
			{
				Console.WriteLine("{0}: {1} meas, {2}", section.Type, section.Measures, string.Join(" - ", section.Chords.Select(c => c.Item2)));
			}
		}

		[TestMethod]
		public void ChordProgressionTest()
		{
			for (var i = 0; i < 50; i++)
			{
				Console.WriteLine(ChordProgressionGenerator.ChordProgression(n => n));
			}
		}

		[TestMethod]
		public void ChordProgressionMinorOrDiminishedOnlyTest()
		{
			for (var i = 0; i < 50; i++)
			{
				Console.WriteLine(ChordProgressionGenerator.ChordProgression(MinorOrDiminshedFilter));
			}
		}

		private static Func<MarkovChainNode<Chord>, MarkovChainNode<Chord>> MinorOrDiminshedFilter
		{
			get
			{
				return n =>
					n.Data.Quality != ChordQuality.Minor && n.Data.Quality != ChordQuality.Diminished
						? new MarkovChainNode<Chord>(n.Data, n.Probability * 0.25, n.ChildNodes?.Where(c => c.Probability > 0.08).ToList())
						: n;
			}
		}
	}
}
