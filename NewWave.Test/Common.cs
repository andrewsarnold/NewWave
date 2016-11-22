﻿using System;
using System.Diagnostics;
using System.IO;
using NewWave.Core;

namespace NewWave.Test
{
	internal static class Common
	{
		private static readonly string Dir = AppDomain.CurrentDomain.BaseDirectory;

		internal static void RenderAndPlay(Parameters parameters, Song song, string fileName)
		{
			Console.WriteLine(song.Generate(parameters));
			var score = song.Render();
			var outputPath = Path.Combine(Dir, fileName);
			score.ExportMidi(outputPath);
			Process.Start(outputPath);
		}
	}
}
