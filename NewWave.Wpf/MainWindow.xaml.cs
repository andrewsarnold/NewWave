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

namespace NewWave.Wpf
{
	public partial class MainWindow
	{
		private BackgroundWorker _generator;
		private BackgroundWorker _renderer;

		private UiParameterList _parameterList;
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
				song.Generate(_parameterList.ToParameterList());
				args.Result = song;
			};
			_generator.RunWorkerCompleted += (o, args) =>
			{
				BtnGenerate.Content = "Generate";
				BtnGenerate.IsEnabled = true;
				_song = (GeneratedSong)args.Result;
				BtnRender.IsEnabled = true;
				TxtGeneratedResults.Text = $"{_song.WriteStats()}\n{_song.WriteSections()}";
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
			_parameterList = new UiParameterList();
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
			_parameterList.Key = ((ComboBoxItem)CmbKey.SelectedItem).Content.ToString();
			_parameterList.Tuning = ((ComboBoxItem)CmbTuning.SelectedItem).Content.ToString();
			_parameterList.TempoMean = SldTempoMean.Value;
			_parameterList.TempoStdDev = SldTempoStdDev.Value;
			_parameterList.LengthMean = SldLengthMean.Value;
			_parameterList.LengthStdDev = SldLengthStdDev.Value;
			_parameterList.MajorAffinity = SldChordMajor.Value;
			_parameterList.MinorAffinity = SldChordMinor.Value;
			_parameterList.DiminishedAffinity = SldChordDim.Value;
		}

		private void LoadParameters()
		{
			CmbTuning.SelectedIndex = _parameterList.Tuning == "Drop" ? 1 : 0;
			for (var i = 0; i < CmbKey.Items.Count; i++)
			{
				if (((ComboBoxItem)CmbKey.Items[i]).Content.ToString() == _parameterList.Key)
				{
					CmbKey.SelectedIndex = i;
					break;
				}
			}

			SldTempoMean.Value = _parameterList.TempoMean;
			SldTempoStdDev.Value = _parameterList.TempoStdDev;
			SldLengthMean.Value = _parameterList.LengthMean;
			SldLengthStdDev.Value = _parameterList.LengthStdDev;
			SldChordMajor.Value = _parameterList.MajorAffinity;
			SldChordMinor.Value = _parameterList.MinorAffinity;
			SldChordDim.Value = _parameterList.DiminishedAffinity;
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
					_parameterList = (UiParameterList)new BinaryFormatter().Deserialize(stream);
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
