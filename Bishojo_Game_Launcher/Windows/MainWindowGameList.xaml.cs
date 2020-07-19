using Bishojo_Game_Launcher.Property;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace Bishojo_Game_Launcher.Windows {
	/// <summary>
	/// MainWindowGameList.xaml の相互作用ロジック
	/// </summary>
	public partial class MainWindowGameList : UserControl {
		private static List<Game.Game.GameDetaile> gamelist;

		public MainWindowGameList() {
			InitializeComponent();
			GameListReload();
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

		public void GameListReload() {
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
			var listBox = sender as ListBox;
			var selectedItem = listBox.Items[listBox.SelectedIndex] as ListItem;
			var detaile = gamelist.Find(x => x.Hash == selectedItem.Hash);
			MainImage.Source = new BitmapImage(
				new Uri(
					AppPath.GamesFolder +
					detaile.Hash + @"\" +
					detaile.Hash + ".jpg"
				)
			);
		}
	}
}
