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
using BishojoGameLauncher.Game;
using BishojoGameLauncher.Properties;

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

		private Dictionary<string, GameDetaile> downloadList = new Dictionary<string, GameDetaile>();

		public void ReloadDownloadList() {
			foreach (var game in GamesSettings.Instance.Games) {
				if (game.Value.DownloadComplete) {
					continue;
				}
				downloadList.Add(game.Value.Hash, game.Value);
				DownloadList.Items.Add(new {
					AppIcon = Imaging.CreateBitmapSourceFromHIcon(
						Icon.ExtractAssociatedIcon(game.Value.ExecutableFile).Handle,
						Int32Rect.Empty,
						BitmapSizeOptions.FromEmptyOptions()
					),
					Title = game.Value.Detaile.Title,
					Brand = game.Value.Detaile.Brand,
				});
			}
		}

		public async void DownloadStart() {
			var downloadImage = new DownloadImage();
			if (Settings.Instance.IsProxyEnable) {
				
				if (Settings.Instance.ProxyType) {
					/// Socks5
					if (Settings.Instance.ProxyUser != default(string)) {
						downloadImage.UseSocks5Proxy(
							Settings.Instance.ProxyHost,
							Settings.Instance.ProxyPort,
							Settings.Instance.ProxyUser,
							Settings.Instance.ProxyPassword
						);
					} else {
						downloadImage.UseSocks5Proxy(
							Settings.Instance.ProxyHost,
							Settings.Instance.ProxyPort
						);
					}
				} else {
					/// HTTP
					if (Settings.Instance.ProxyUser != default(string)) {
						downloadImage.UseHTTPProxy(
							Settings.Instance.ProxyHost,
							Settings.Instance.ProxyPort,
							Settings.Instance.ProxyUser,
							Settings.Instance.ProxyPassword
						);
					} else {
						downloadImage.UseHTTPProxy(
							Settings.Instance.ProxyHost,
							Settings.Instance.ProxyPort
						);
					}
				}
			}

			foreach (var game in downloadList) {
				await downloadImage.Run(game.Value);
				GamesSettings.Instance.Games[game.Key].DownloadComplete = true;
			}
			GamesSettings.Instance.Save();
		}
	}
}
