using BishojoGameLauncher.Game;
using BishojoGameLauncher.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BishojoGameLauncher.Windows {
	/// <summary>
	/// MainWindowGameList.xaml の相互作用ロジック
	/// </summary>
	public partial class MainWindowGameList : UserControl {
		private static GameDetaile selectedGameDetaile;

		public MainWindowGameList() {
			InitializeComponent();
			ReloadGameList();
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

		public void ReloadGameList() {
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
			GameList.SelectedIndex = 0;
		}

		private void GameList_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			Title.Text = "";
			Brand.Text = "";
			ReleseData.Text = "";
			MainImage.Source = null;
			Illustrator.Items.Clear();
			Scenarios.Items.Clear();
			Composer.Items.Clear();
			VoiceActor.Items.Clear();
			Singer.Items.Clear();
			var listBox = sender as ListBox;
			var selectedItem = listBox.Items[listBox.SelectedIndex] as ListItem;
			selectedGameDetaile = GamesSettings.Instance.Games[selectedItem.Hash];

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

			try {
				MainImage.Source = new BitmapImage(
					new Uri(
						AppPath.GamesFolder +
						selectedGameDetaile.Hash + @"\" +
						selectedGameDetaile.Hash + Path.GetExtension(selectedGameDetaile.Detaile.MainImage)
					)
				);
			} catch {
				return;
			}
		}

		private void SearchIcon_Click(object sender, RoutedEventArgs e) {
			GameSearchWord.Focus();
		}

		private void Play_Click(object sender, RoutedEventArgs e) {
			var processInfo = new ProcessStartInfo();
			processInfo.FileName = selectedGameDetaile.ExecutableFile;
			Process.Start(processInfo);
		}
	}
}
