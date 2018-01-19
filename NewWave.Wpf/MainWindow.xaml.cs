using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using NewWave.Generator;
using NewWave.Generator.Parameters;

namespace NewWave.Wpf
{
	public partial class MainWindow
	{
		private BackgroundWorker _backgroundWorker;
		private readonly string _exportFolder = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyMusic)}\NewWave Export";

		public MainWindow()
		{
			InitializeComponent();
			Title = $"{FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductName} {Assembly.GetExecutingAssembly().GetName().Version}";
			InitializeGenerator();
		}

		private void InitializeGenerator()
		{
			// Set up background worker
			_backgroundWorker = new BackgroundWorker();
			_backgroundWorker.DoWork += (o, args) =>
			{
				// Create export folder
				if (!Directory.Exists(_exportFolder))
				{
					Directory.CreateDirectory(_exportFolder);
				}

				var song = new GeneratedSong();
				song.Generate(new ParameterList());
				var score = song.Render();
				var outputPath = $@"{_exportFolder}\{DateTime.Now:yyyyMMddhhmmss}.mid";
				score.ExportMidi(outputPath);
				args.Result = outputPath;
			};

			_backgroundWorker.RunWorkerCompleted += (o, args) =>
			{
				BtnExport.Content = "Export";
				BtnExport.IsEnabled = true;

				if (ChkPlay.IsChecked ?? false)
				{
					Process.Start(args.Result.ToString());
				}
			};
		}

		private void Export(object sender, RoutedEventArgs e)
		{
			BtnExport.Content = "Exporting...";
			BtnExport.IsEnabled = false;
			_backgroundWorker.RunWorkerAsync();
		}
	}
}
