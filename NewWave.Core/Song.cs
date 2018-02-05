namespace NewWave.Core
{
	/// <summary>
	/// An abstract-ish set of instructions on how a song is written. Kind of like a vector image file.
	///  From here the user can easily make high-level adjustments to the arrangement.
	/// </summary>
	public abstract class Song
	{
		/// <summary>
		/// Generates the song.
		/// Any input parameters would be collected by the Song implementation's constructor.
		/// </summary>
		public abstract void Generate(IParameterList parameterList);

		/// <summary>
		/// Renders a "vector" song into a "raster" score. Does not involve any random
		/// generation or guesswork; all necessary instructions should be included
		/// in the song itself.
		/// </summary>
		/// <returns>A score based on the given song.</returns>
		public abstract Score Render();

		/// <summary>
		/// The name of the song engine, for display in the GUI.
		/// </summary>
		public abstract string DisplayName { get; }

		public override string ToString()
		{
			return DisplayName;
		}
	}
}
