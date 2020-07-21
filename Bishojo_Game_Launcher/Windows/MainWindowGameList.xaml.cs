using Bishojo_Game_Launcher.Property;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Bishojo_Game_Launcher.Windows {
	/// <summary>
	/// MainWindowGameList.xaml の相互作用ロジック
	/// </summary>
	public partial class MainWindowGameList : UserControl {
		private static List<Game.Game.GameDetaile> gamelist;

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
			var list = new Game.List();
			list.Read();
			gamelist = list.Games;
			foreach (var game in gamelist) {
				GameList.Items.Add(
					new ListItem(
						game.Hash,
						Imaging.CreateBitmapSourceFromHIcon(
							Icon.ExtractAssociatedIcon(game.ExecutableFile).Handle,
							Int32Rect.Empty,
							BitmapSizeOptions.FromEmptyOptions()
						),
						game.Detaile.Title
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
			var detaile = gamelist.Find(x => x.Hash == selectedItem.Hash);

			Title.Text = detaile.Detaile.Title;
			Brand.Text = detaile.Detaile.Brand;
			ReleseData.Text = detaile.Detaile.Sellday;
			foreach (var illustrator in detaile.Detaile.Illustrators) {
				Illustrator.Items.Add(illustrator);
			}
			foreach (var scenarios in detaile.Detaile.Scenarios) {
				Scenarios.Items.Add(scenarios);
			}
			foreach (var composer in detaile.Detaile.Composers) {
				Composer.Items.Add(composer);
			}
			for (var i = 0; i < detaile.Detaile.Voices.Count; i++) {
				VoiceActor.Items.Add(
					new {
						Voice = detaile.Detaile.Voices[i],
						Character = detaile.Detaile.Characters[i]
					}
				);
			}
			foreach (var singer in detaile.Detaile.Singers) {
				Singer.Items.Add(singer);
			}

			try {
				MainImage.Source = new BitmapImage(
					new Uri(
						AppPath.GamesFolder +
						detaile.Hash + @"\" +
						detaile.Hash + Path.GetExtension(detaile.Detaile.MainImage)
					)
				);
			} catch {
				return;
			}
		}
	}
}
