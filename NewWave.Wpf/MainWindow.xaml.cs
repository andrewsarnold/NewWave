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
		private BackgroundWorker _generator;
		private BackgroundWorker _renderer;
		private GeneratedSong _song;

		private readonly string _exportFolder = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyMusic)}\NewWave Export";

		public MainWindow()
		{
			InitializeComponent();
			Title = $"{FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductName} {Assembly.GetExecutingAssembly().GetName().Version}";
			InitializeGenerators();
		}

		private void InitializeGenerators()
		{
			_generator = new BackgroundWorker();
			_generator.DoWork += (o, args) =>
			{
				// Create export folder
				if (!Directory.Exists(_exportFolder))
				{
					Directory.CreateDirectory(_exportFolder);
				}

				var song = new GeneratedSong();
				song.Generate(new ParameterList());
				args.Result = song;
			};
			_generator.RunWorkerCompleted += (o, args) =>
			{
				BtnGenerate.Content = "Generate";
				BtnGenerate.IsEnabled = true;
				_song = (GeneratedSong)args.Result;
				BtnRender.IsEnabled = true;
			};

			_renderer = new BackgroundWorker();
			_renderer.DoWork += (o, args) =>
			{
				var score = _song.Render();
				var outputPath = $@"{_exportFolder}\{DateTime.Now:yyyyMMddhhmmss}.mid";
				score.ExportMidi(outputPath);
				args.Result = outputPath;
			};
			_renderer.RunWorkerCompleted += (o, args) =>
			{
				BtnRender.Content = "Render";
				BtnRender.IsEnabled = true;
				if (ChkPlay.IsChecked ?? false)
				{
					Process.Start(args.Result.ToString());
				}
			};
		}

		private void Generate(object sender, RoutedEventArgs e)
		{
			BtnGenerate.Content = "Generating...";
			BtnGenerate.IsEnabled = false;
			_generator.RunWorkerAsync();
		}

		private void Render(object sender, RoutedEventArgs e)
		{
			BtnRender.Content = "Rendering...";
			BtnRender.IsEnabled = false;
			_renderer.RunWorkerAsync();
		}
	}
}
