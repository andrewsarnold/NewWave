using System.Collections.Generic;
using System.Linq;
using NewWave.Library.Pitches;
using NewWave.Midi;

namespace NewWave.Library.Chords
{
	public struct Chord
	{
		public Pitch BasePitch;
	    public readonly ChordQuality Quality;
		public readonly bool IsInverted;
		public Pitch Inversion;

		private readonly ChordAdded _added;
		
		public Chord(Pitch basePitch, ChordQuality quality = ChordQuality.NotSpecified, ChordAdded added = ChordAdded.None)
		{
			BasePitch = basePitch;
			Quality = quality;
			_added = added;
			IsInverted = false;
			Inversion = Pitch.A;
		}

		public Chord(Pitch basePitch, ChordQuality quality, ChordAdded added, Pitch inversion)
		{
			BasePitch = basePitch;
			Quality = quality;
			_added = added;
			Inversion = inversion;
			IsInverted = true;
		}

		public IEnumerable<MidiPitch> PitchesHigherThan(MidiPitch lowestPossiblePitch)
		{
			var pitches = Pitches(-1);
			while (pitches.Any(p => p < lowestPossiblePitch))
			{
				pitches = pitches.Select(p => p + 12);
			}
			return pitches;
		}

		public IEnumerable<MidiPitch> Pitches(int rootOctave)
		{
			var basePitch = BasePitch.ToMidiPitch(rootOctave);
			var pitches = new List<MidiPitch> { (IsInverted ? Inversion : BasePitch).ToMidiPitch(rootOctave) };

			// Second note
			if (Quality == ChordQuality.Minor || Quality == ChordQuality.Diminished)
			{
				pitches.Add(basePitch + 3);
			}
			else if (Quality == ChordQuality.Suspended)
			{
				pitches.Add(basePitch + 5);
			}
			else
			{
				pitches.Add(basePitch + 4);
			}

			// Third note
			pitches.Add(basePitch + 7);

			// Additional notes
			if (_added != ChordAdded.None)
			{
				switch (_added)
				{
					case ChordAdded.Six:
						pitches.Add(basePitch + 9);
						break;
					case ChordAdded.Seven:
						pitches.Add(basePitch + (Quality == ChordQuality.Major ? 11 : 10));
						break;
					case ChordAdded.Nine:
						pitches.Add(basePitch + (Quality == ChordQuality.Major ? 11 : 10));
						pitches.Add(basePitch + (Quality == ChordQuality.Minor ? 13 : 14));
						break;
					case ChordAdded.Eleven:
						pitches.Add(basePitch + (Quality == ChordQuality.Major ? 11 : 10));
						pitches.Add(basePitch + (Quality == ChordQuality.Minor ? 13 : 14));
						pitches.Add(basePitch + (Quality == ChordQuality.Diminished ? 16 : 17));
						break;
				}
			}

			return pitches;
		}

		public void Transpose(int halfsteps)
		{
			BasePitch = (Pitch)(((int)BasePitch + halfsteps) % 12);
			Inversion = (Pitch)(((int)Inversion + halfsteps) % 12);
		}

		public override bool Equals(object obj)
		{
			if (!(obj is Chord)) return false;

			var other = (Chord)obj;
			return
				BasePitch == other.BasePitch &&
				_added == other._added &&
				IsInverted == other.IsInverted &&
				(!IsInverted || Inversion == other.Inversion) &&
				Quality == other.Quality;
		}

		public override string ToString()
		{
			var quality = string.Empty;
			switch (Quality)
			{
				case ChordQuality.Major:
					quality = "maj";
					break;
				case ChordQuality.Minor:
					quality = "min";
					break;
				case ChordQuality.Augmented:
					quality = "aug";
					break;
				case ChordQuality.Diminished:
					quality = "dim";
					break;
			}

			var added = string.Empty;
			switch (_added)
			{
				case ChordAdded.Six:
					added = "6";
					break;
				case ChordAdded.Seven:
					added = "7";
					break;
				case ChordAdded.Nine:
					added = "9";
					break;
				case ChordAdded.Eleven:
					added = "11";
					break;
			}

			var inverted = IsInverted ? string.Format("/{0}", Inversion.NoteName()) : string.Empty;

			return string.Format("{0}{1}{2}{3}", BasePitch.NoteName(), quality, added, inverted);
		}
	}
}
