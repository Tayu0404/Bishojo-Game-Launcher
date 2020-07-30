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

namespace BishojoGameLauncher.Windows {
	/// <summary>
	/// MainWindowDownloadList.xaml の相互作用ロジック
	/// </summary>
	public partial class MainWindowDownloadList : UserControl {
		public MainWindowDownloadList() {
			InitializeComponent();
			ReloadDownloadList();
			DownloadStart();
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

		public async void DownloadStart() {
			var list = new Game.List();
			list.Read();
			var downloadImage = new Game.DownloadImage();

			if (Properties.Settings.Default.IsProxyEnable) {
				
				if (Properties.Settings.Default.ProxyType) {
					/// Socks5
					if (Properties.Settings.Default.ProxyUser != default(string)) {
						downloadImage.UseSocks5Proxy(
							Properties.Settings.Default.ProxyHost,
							Properties.Settings.Default.ProxyPort,
							Properties.Settings.Default.ProxyUser,
							Properties.Settings.Default.ProxyPassword
						);
					} else {
						downloadImage.UseSocks5Proxy(
							Properties.Settings.Default.ProxyHost,
							Properties.Settings.Default.ProxyPort
						);
					}
				} else {
					/// HTTP
					if (Properties.Settings.Default.ProxyUser != default(string)) {
						downloadImage.UseHTTPProxy(
							Properties.Settings.Default.ProxyHost,
							Properties.Settings.Default.ProxyPort,
							Properties.Settings.Default.ProxyUser,
							Properties.Settings.Default.ProxyPassword
						);
					} else {
						downloadImage.UseHTTPProxy(
							Properties.Settings.Default.ProxyHost,
							Properties.Settings.Default.ProxyPort
						);
					}
				}
			}

			foreach (var game in downloadList) {
				await downloadImage.Run(game);
				list.Games.Find(x => x.Hash == game.Hash).DownloadComplete = true;
			}
			list.Write();
		}
	}
}
