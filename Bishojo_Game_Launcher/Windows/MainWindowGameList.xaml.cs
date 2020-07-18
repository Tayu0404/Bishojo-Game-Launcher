using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace Bishojo_Game_Launcher.Windows {
	/// <summary>
	/// MainWindowGameList.xaml の相互作用ロジック
	/// </summary>
	public partial class MainWindowGameList : UserControl {
		private static List<Game.List.GameDetaile> gamelist;

		public MainWindowGameList() {
			InitializeComponent();
			GameListReload();
		}

		public void GameListReload() {
			GameList.Items.Clear();
			var list = new Game.List();
			list.Read();
			gamelist = list.Games;
			foreach (var game in gamelist) {
				GameList.Items.Add(new {
					Hash = game.Hash,
					AppIcon = Imaging.CreateBitmapSourceFromHIcon(
						Icon.ExtractAssociatedIcon(game.ExecutableFile).Handle,
						Int32Rect.Empty,
						BitmapSizeOptions.FromEmptyOptions()
					),
					Title = game.Detaile.Title });
			}
			GameList.SelectedItem = 0;
		}
	}
}
