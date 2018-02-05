using System;
using System.Diagnostics;
using System.IO;
using NewWave.Core;

namespace NewWave.Test
{
	internal static class Common
	{
		private static readonly string Dir = AppDomain.CurrentDomain.BaseDirectory;

		internal static void RenderAndPlay(IParameterList parameterList, Song song, string fileName)
		{
			song.Generate(parameterList);
			var score = song.Render();
			var outputPath = Path.Combine(Dir, fileName);
			score.ExportMidi(outputPath);
			Process.Start(outputPath);
		}
	}
}
