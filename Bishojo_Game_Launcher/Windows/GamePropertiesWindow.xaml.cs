using AngleSharp;
using BishojoGameLauncher.Properties;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BishojoGameLauncher.Windows {
	/// <summary>
	/// GamePropertiesWindow.xaml の相互作用ロジック
	/// </summary>
	public partial class GamePropertiesWindow : Window {
		public GamePropertiesWindow(string hash) {
			InitializeComponent();
			Hash = hash;
			var game = GamesSettings.Instance.Games[Hash];
			WindowTitle.Text = makeWindowTitle(game.Detaile.Title);
			if (game.Detaile.Web != "") {
				HPURL.NavigateUri = new Uri(game.Detaile.Web);
			}
			HPTitle.Text = game.Detaile.Title;
			StartOptions.Text = game.StartOptions;
		}

		private void WindowClosing(object sender, CancelEventArgs e) {
			try {
				if (StartOptions.Text != GamesSettings.Instance.Games[Hash].StartOptions) {
					GamesSettings.Instance.Games[Hash].StartOptions = StartOptions.Text;
					GamesSettings.Instance.Save();
				}
			} catch (KeyNotFoundException) {
				return;
			}
		}

		private string Hash { get; set; }

		private string makeWindowTitle(string gameTitle) {
			var maxLength = 15;
			if (gameTitle.Length > maxLength) {
				gameTitle = $"{gameTitle.Substring(0, maxLength)}...";
			}
			var windowTitle = $"{gameTitle} - {Properties.Resources.Properties}";
			return windowTitle;
		}

		private void WindowMiniMize_Click(object sender, RoutedEventArgs e) {
			this.WindowState = WindowState.Minimized;
		}

		private void WindowClose_Click(object sender, RoutedEventArgs e) {
			this.Close();
		}

		private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e) {
			Process.Start(e.Uri.ToString());
		}

		private void OpenGameFolder_Click(object sender, RoutedEventArgs e) {
			var path = GamesSettings.Instance.Games[Hash].ExecutableFile;
			var processInfo = new ProcessStartInfo();
			processInfo.FileName = "explorer.exe";
			processInfo.Arguments = path.Substring(0, path.LastIndexOf(@"\") + 1);
			Process.Start(processInfo);
		}

		private void OpenSaveDataFolder_Click(object sender, RoutedEventArgs e) {
			var processInfo = new ProcessStartInfo();
			processInfo.FileName = "explorer.exe";
			processInfo.Arguments = GamesSettings.Instance.Games[Hash].SaveFolder;
			Process.Start(processInfo);
		}

		private void ChangeIcon_Click(object sender, RoutedEventArgs e) {
			var dialog = new CommonOpenFileDialog(Properties.Resources.ExecutableFile);

			dialog.Filters.Add(new CommonFileDialogFilter("ICO File", "*.ico"));
			dialog.Filters.Add(new CommonFileDialogFilter("PNG File", "*.png"));
			dialog.Filters.Add(new CommonFileDialogFilter("JPG File", "*.png"));
			dialog.Filters.Add(new CommonFileDialogFilter("All File", "*.*"));

			var path = GamesSettings.Instance.Games[Hash].ExecutableFile;
			dialog.InitialDirectory = path.Substring(0, path.LastIndexOf(@"\") + 1);

			if (dialog.ShowDialog() == CommonFileDialogResult.Ok) {
				var filename = dialog.FileName;
				try {
					new BitmapImage(new Uri(filename));
				} catch {
					return;
				}
				GamesSettings.Instance.Games[Hash].CustomaIconPath = filename;
				GamesSettings.Instance.Save();
				OnChangeIcon(filename);
			}
		}
		protected virtual void OnChangeIcon(string e) {
			var handler = _ChangeIcon;
			if (handler != null) {
				handler(this, new ChangeIconEventArgs(e));
			}
		}

		public class ChangeIconEventArgs : EventArgs {
			private readonly string _changeIconPath;

			public ChangeIconEventArgs(string changeIconPath) {
				this._changeIconPath = changeIconPath;
			}

			public string ChangeIconPath {
				get { return this._changeIconPath; }
			}
		}

		public delegate void ChangeIconEventHandler(
			object sender,
			ChangeIconEventArgs args
		);

		private event ChangeIconEventHandler _ChangeIcon;

		public event ChangeIconEventHandler ChangeIcon {
			add { _ChangeIcon += value; }
			remove { _ChangeIcon -= value; }
		}

		private void StartOptions_Click(object sender, RoutedEventArgs e) {
			StartOptions.Focus();
		}

		protected virtual void OnDeleteGame(EventArgs e) {
			var handler = _DeleteGame;
			if (handler != null) {
				handler(this, e);
			}
		}

		private event EventHandler _DeleteGame;

		public event EventHandler DeleteGame {
			add { _DeleteGame += value; }
			remove { _DeleteGame -= value; }
		}

		private void Delete_Click(object sender, RoutedEventArgs e) {
			GamesSettings.Instance.Games.Remove(Hash);
			GamesSettings.Instance.Save();
			OnDeleteGame(EventArgs.Empty);
			this.Close();
		}
	}
}
