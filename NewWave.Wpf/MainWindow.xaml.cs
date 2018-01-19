using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using NewWave.Generator;
using NewWave.Generator.Parameters;

namespace NewWave.Wpf
{
	public partial class MainWindow
	{
		public MainWindow()
		{
			InitializeComponent();
			Title = $"{FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductName} {Assembly.GetExecutingAssembly().GetName().Version}";
		}

		private void Export(object sender, RoutedEventArgs e)
		{
			var song = new GeneratedSong();
			song.Generate(new ParameterList());
			var score = song.Render();
			var outputPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyMusic)}\{DateTime.Now:yyyyMMddhhmmss}.mid";
			score.ExportMidi(outputPath);
			Process.Start(outputPath);
		}
	}
}
