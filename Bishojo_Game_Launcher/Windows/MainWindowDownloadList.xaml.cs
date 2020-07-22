using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Bishojo_Game_Launcher.Windows {
	/// <summary>
	/// MainWindowDownloadList.xaml の相互作用ロジック
	/// </summary>
	public partial class MainWindowDownloadList : UserControl {
		public MainWindowDownloadList() {
			InitializeComponent();
			ReloadDownloadList();
			//DownloadStart();
		}

		private List<Game.Game.GameDetaile> downloadList = new List<Game.Game.GameDetaile>();

		public void ReloadDownloadList() {
			var list = new Game.List();
			list.Read();
			foreach (var game in list.Games) {
				if (game.DownloadComplete) {
					continue;
				}
				downloadList.Add(game);
				DownloadList.Items.Add(new {
					AppIcon = Imaging.CreateBitmapSourceFromHIcon(
						Icon.ExtractAssociatedIcon(game.ExecutableFile).Handle,
						Int32Rect.Empty,
						BitmapSizeOptions.FromEmptyOptions()
					),
					Title = game.Detaile.Title,
					Brand = game.Detaile.Brand,
				});
			}
		}

		public void DownloadStart() {
			Task.Run(async() => {
				var list = new Game.List();
				list.Read();
				foreach (var game in downloadList) {
					await Game.Game.Downlaod(game);
					list.Games.Find(x => x.Hash == game.Hash).DownloadComplete = true;
				}
				list.Write();
			});
		}
	}
}
