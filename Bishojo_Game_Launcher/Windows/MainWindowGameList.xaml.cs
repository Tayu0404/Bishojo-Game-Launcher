using BishojoGameLauncher.Game;
using BishojoGameLauncher.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BishojoGameLauncher.Windows {
	/// <summary>
	/// MainWindowGameList.xaml の相互作用ロジック
	/// </summary>
	public partial class MainWindowGameList : UserControl {

		public MainWindowGameList() {
			InitializeComponent();
			Reload();
			GameList.SelectedIndex = 0;
		}
		private class ListItem {
			public ListItem(string hash, BitmapSource appIcon, string title) {
				this.Hash = hash;
				this.AppIcon = appIcon;
				this.Title = title;
			}	

			public string Hash { get; private set; }

			public BitmapSource AppIcon { get; private set; }

			public string Title { get; private set; }
		}

		public void Reload() {
			GameList.Items.Clear();
			var games = GamesSettings.Instance.Games;
			foreach (var game in games) {
				GameList.Items.Add(
					new ListItem(
						game.Value.Hash,
						Imaging.CreateBitmapSourceFromHIcon(
							Icon.ExtractAssociatedIcon(game.Value.ExecutableFile).Handle,
							Int32Rect.Empty,
							BitmapSizeOptions.FromEmptyOptions()
						),
						game.Value.Detaile.Title
					)
				);
			}
		}

		public void Add(string hash, string executableFile, string title ) {
			GameList.Items.Add(
				new ListItem(
					hash,
					Imaging.CreateBitmapSourceFromHIcon(
						Icon.ExtractAssociatedIcon(executableFile).Handle,
						Int32Rect.Empty,
						BitmapSizeOptions.FromEmptyOptions()
					),
					title
				)
			);
		}

		private void GameList_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			var listBox = sender as ListBox;
			if (listBox.SelectedIndex == -1) {
				return;
			}

			Title.Text = "";
			Brand.Text = "";
			ReleseData.Text = "";
			MainImage.Source = null;
			Illustrator.Items.Clear();
			Scenarios.Items.Clear();
			Composer.Items.Clear();
			VoiceActor.Items.Clear();
			Singer.Items.Clear();

			var selectedItem = listBox.Items[listBox.SelectedIndex] as ListItem;
			var selectedGameDetaile = GamesSettings.Instance.Games[selectedItem.Hash];

			Title.Text = selectedGameDetaile.Detaile.Title;
			Brand.Text = selectedGameDetaile.Detaile.Brand;
			ReleseData.Text = selectedGameDetaile.Detaile.Sellday;
			foreach (var illustrator in selectedGameDetaile.Detaile.Illustrators) {
				Illustrator.Items.Add(illustrator);
			}
			foreach (var scenarios in selectedGameDetaile.Detaile.Scenarios) {
				Scenarios.Items.Add(scenarios);
			}
			foreach (var composer in selectedGameDetaile.Detaile.Composers) {
				Composer.Items.Add(composer);
			}
			for (var i = 0; i < selectedGameDetaile.Detaile.Voices.Count; i++) {
				VoiceActor.Items.Add(
					new {
						Voice = selectedGameDetaile.Detaile.Voices[i],
						Character = selectedGameDetaile.Detaile.Characters[i]
					}
				);
			}
			foreach (var singer in selectedGameDetaile.Detaile.Singers) {
				Singer.Items.Add(singer);
			}

			if (selectedGameDetaile.DownloadComplete) {
				MainImage.Source = new BitmapImage(
					new Uri(
						AppPath.GamesFolder +
						selectedGameDetaile.Hash + @"\" +
						selectedGameDetaile.Hash + Path.GetExtension(selectedGameDetaile.Detaile.MainImage)
					)
				);
			}
		}

		private void SearchIcon_Click(object sender, RoutedEventArgs e) {
			GameSearchWord.Focus();
		}

		private void Play_Click(object sender, RoutedEventArgs e) {
			var selectedGameDetaile = GameList.SelectedItem as ListItem;
			var processInfo = new ProcessStartInfo();
			processInfo.FileName = GamesSettings.Instance.Games[selectedGameDetaile.Hash].ExecutableFile;
			Process.Start(processInfo);
		}

		private void Delete_Click(object sender, RoutedEventArgs e) {
			var selectedIndex = GameList.SelectedIndex;
			var selectedGameDetaile = GameList.SelectedItem as ListItem;
			GamesSettings.Instance.Games.Remove(selectedGameDetaile.Hash);
			GamesSettings.Instance.Save();
			Reload();
			if (GameList.Items.Count == 0) {
				return;
			}
			GameList.SelectedIndex = selectedIndex - 1;
		}
	}
}
