using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NewWave.Library.Pitches;
using NewWave.Library.Scales;
using NewWave.Midi;

namespace NewWave.Test.LibraryTests
{
	[TestClass]
	public class ScaleTests
	{
		[TestMethod]
		public void MajorScaleTest()
		{
			var scale = ScaleLibrary.GetScale(Pitch.D, ScaleType.Major).ToList();
			Assert.AreEqual(7, scale.Count);
			Assert.AreEqual(Pitch.D, scale[0]);
			Assert.AreEqual(Pitch.E, scale[1]);
			Assert.AreEqual(Pitch.FSharp, scale[2]);
			Assert.AreEqual(Pitch.G, scale[3]);
			Assert.AreEqual(Pitch.A, scale[4]);
			Assert.AreEqual(Pitch.B, scale[5]);
			Assert.AreEqual(Pitch.CSharp, scale[6]);
		}

		[TestMethod]
		public void MinorScaleTest()
		{
			var scale = ScaleLibrary.GetScale(Pitch.C, ScaleType.Minor).ToList();
			Assert.AreEqual(7, scale.Count);
			Assert.AreEqual(Pitch.C, scale[0]);
			Assert.AreEqual(Pitch.D, scale[1]);
			Assert.AreEqual(Pitch.DSharp, scale[2]);
			Assert.AreEqual(Pitch.F, scale[3]);
			Assert.AreEqual(Pitch.G, scale[4]);
			Assert.AreEqual(Pitch.GSharp, scale[5]);
			Assert.AreEqual(Pitch.ASharp, scale[6]);
		}

		[TestMethod]
		public void MajorPentatonicScaleTest()
		{
			var scale = ScaleLibrary.GetScale(Pitch.C, ScaleType.MajorPentatonic).ToList();
			Assert.AreEqual(5, scale.Count);
			Assert.AreEqual(Pitch.C, scale[0]);
			Assert.AreEqual(Pitch.D, scale[1]);
			Assert.AreEqual(Pitch.E, scale[2]);
			Assert.AreEqual(Pitch.G, scale[3]);
			Assert.AreEqual(Pitch.A, scale[4]);
		}

		[TestMethod]
		public void MinorPentatonicScaleTest()
		{
			var scale = ScaleLibrary.GetScale(Pitch.C, ScaleType.MinorPentatonic).ToList();
			Assert.AreEqual(5, scale.Count);
			Assert.AreEqual(Pitch.C, scale[0]);
			Assert.AreEqual(Pitch.DSharp, scale[1]);
			Assert.AreEqual(Pitch.F, scale[2]);
			Assert.AreEqual(Pitch.G, scale[3]);
			Assert.AreEqual(Pitch.ASharp, scale[4]);
		}

		[TestMethod]
		public void MajorScaleStepTest()
		{
			Assert.AreEqual(MidiPitch.E0, ScaleLibrary.Step(Pitch.C, ScaleType.Major, MidiPitch.A0, -3));
			Assert.AreEqual(MidiPitch.GNeg1, ScaleLibrary.Step(Pitch.C, ScaleType.Major, MidiPitch.C0, -3));
			Assert.AreEqual(MidiPitch.D1, ScaleLibrary.Step(Pitch.C, ScaleType.Major, MidiPitch.A0, 3));
			Assert.AreEqual(MidiPitch.A1, ScaleLibrary.Step(Pitch.C, ScaleType.Major, MidiPitch.A0, 7));
			Assert.AreEqual(MidiPitch.D1, ScaleLibrary.Step(Pitch.C, ScaleType.Major, MidiPitch.B0, 2));
		}

		[TestMethod]
		public void MinorScaleStepTest()
		{
			Assert.AreEqual(MidiPitch.GSharpNeg1, ScaleLibrary.Step(Pitch.C, ScaleType.Minor, MidiPitch.D0, -3));
			Assert.AreEqual(MidiPitch.D4, ScaleLibrary.Step(Pitch.C, ScaleType.Minor, MidiPitch.G4, -3));
			Assert.AreEqual(MidiPitch.F2, ScaleLibrary.Step(Pitch.C, ScaleType.Minor, MidiPitch.DSharp2, 1));

			Assert.AreEqual(MidiPitch.D1, ScaleLibrary.Step(Pitch.E, ScaleType.Minor, MidiPitch.G1, -3));
			Assert.AreEqual(MidiPitch.C3, ScaleLibrary.Step(Pitch.E, ScaleType.Minor, MidiPitch.B3, -6));
			Assert.AreEqual(MidiPitch.FSharp5, ScaleLibrary.Step(Pitch.E, ScaleType.Minor, MidiPitch.D5, 2));
		}
	}
}
