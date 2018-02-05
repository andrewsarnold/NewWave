using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using NewWave.Generator;
using NewWave.Generator.Parameters;
using NewWave.Library.Pitches;
using NewWave.Library.Tunings;

namespace NewWave.Wpf
{
	public partial class MainWindow
	{
		private BackgroundWorker _generator;
		private BackgroundWorker _renderer;

		private ParameterList _parameterList;
		private GeneratedSong _song;

		private readonly string _exportFolder = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyMusic)}\NewWave Export";

		public MainWindow()
		{
			InitializeComponent();
			Title = $"{FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductName} {Assembly.GetExecutingAssembly().GetName().Version}";
			InitializeGenerators();
			InitializeParameters();
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
				song.Generate(_parameterList);
				args.Result = song;
			};
			_generator.RunWorkerCompleted += (o, args) =>
			{
				BtnGenerate.Content = "Generate";
				BtnGenerate.IsEnabled = true;
				_song = (GeneratedSong)args.Result;
				BtnRender.IsEnabled = true;
				TxtGeneratedResults.Text = _song.WriteStats();
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

		private void InitializeParameters()
		{
			_parameterList = new ParameterList();
		}

		private void Generate(object sender, RoutedEventArgs e)
		{
			BtnGenerate.Content = "Generating...";
			BtnGenerate.IsEnabled = false;
			SetParameters();
			_generator.RunWorkerAsync();
		}

		private void Render(object sender, RoutedEventArgs e)
		{
			BtnRender.Content = "Rendering...";
			BtnRender.IsEnabled = false;
			_renderer.RunWorkerAsync();
		}

		private void SetParameters()
		{
			var isDropTuning = ((ComboBoxItem)CmbTuning.SelectedItem).Content.ToString() == "Drop";
			var key = PitchExtensions.FromString(((ComboBoxItem)CmbKey.SelectedItem).Content.ToString());
			_parameterList.MinorKeyFunc = () => key.ToMidiPitch(3);
			_parameterList.GuitarTuning = GuitarTuningLibrary.FromPitch(key, isDropTuning);
			_parameterList.BassTuning = _parameterList.GuitarTuning.ToBassTuning();
			_parameterList.TempoMean = SldTempoMean.Value;
			_parameterList.TempoStandardDeviation = SldTempoStdDev.Value;
			_parameterList.LengthInSecondsMean = SldLengthMean.Value;
			_parameterList.LengthInSecondsStandardDeviation = SldLengthStdDev.Value;
		}

		private void LoadParameters()
		{
			CmbTuning.SelectedIndex = _parameterList.GuitarTuning.IsDropTuning ? 1 : 0;
			switch (_parameterList.GuitarTuning.Pitches[0].FromMidiPitch())
			{
				case Pitch.A:
					CmbKey.SelectedIndex = 7;
					break;
				case Pitch.ASharp:
					CmbKey.SelectedIndex = 6;
					break;
				case Pitch.B:
					CmbKey.SelectedIndex = 5;
					break;
				case Pitch.C:
					CmbKey.SelectedIndex = 4;
					break;
				case Pitch.CSharp:
					CmbKey.SelectedIndex = 3;
					break;
				case Pitch.D:
					CmbKey.SelectedIndex = 2;
					break;
				case Pitch.DSharp:
					CmbKey.SelectedIndex = 1;
					break;
				case Pitch.E:
					CmbKey.SelectedIndex = 0;
					break;
				case Pitch.F:
					CmbKey.SelectedIndex = 11;
					break;
				case Pitch.FSharp:
					CmbKey.SelectedIndex = 10;
					break;
				case Pitch.G:
					CmbKey.SelectedIndex = 9;
					break;
				case Pitch.GSharp:
					CmbKey.SelectedIndex = 8;
					break;
			}

			SldTempoMean.Value = _parameterList.TempoMean;
			SldTempoStdDev.Value = _parameterList.TempoStandardDeviation;
			SldLengthMean.Value = _parameterList.LengthInSecondsMean;
			SldLengthStdDev.Value = _parameterList.LengthInSecondsStandardDeviation;
		}

		private void LoadParams(object sender, RoutedEventArgs e)
		{
			var dialog = new OpenFileDialog
			{
				FileName = "params",
				DefaultExt = ".nwp",
				Filter = "NewWave parameters (*.nwp)|*.nwp"
			};

			if (dialog.ShowDialog() ?? false)
			{
				using (Stream stream = File.Open(dialog.FileName, FileMode.Open))
				{
					_parameterList = (ParameterList)new BinaryFormatter().Deserialize(stream);
				}
				LoadParameters();
			}
		}

		private void SaveParams(object sender, RoutedEventArgs e)
		{
			var dialog = new SaveFileDialog
			{
				FileName = "params",
				DefaultExt = ".nwp",
				Filter = "NewWave parameters (*.nwp)|*.nwp"
			};

			if (dialog.ShowDialog() ?? false)
			{
				SetParameters();
				using (var stream = File.Open(dialog.FileName, FileMode.Create))
				{
					new BinaryFormatter().Serialize(stream, _parameterList);
				}
			}
		}
	}
}
