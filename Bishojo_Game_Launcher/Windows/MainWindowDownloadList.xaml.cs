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
		}

		private void initialize() {
			if (
				GamesSettings.Instance.Games == null ||
				GamesSettings.Instance.Games.Count == 0
			) {
				return;
			}
			Reload();
		}

		private Dictionary<string, downloadDetaile> downloadList = new Dictionary<string, downloadDetaile>();

		private class downloadDetaile {
			public downloadDetaile(GameDetaile gameDetaile, bool downloading = false) {
				this.GameDetaile = gameDetaile;
				this.Downloading = downloading;
			}

			public GameDetaile GameDetaile { get; private set; }

			public bool Downloading { get; set; }
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
			foreach (var game in GamesSettings.Instance.Games) {
				if (game.Value.DownloadComplete) {
					continue;
				}
				downloadList.Add(game.Value.Hash, new downloadDetaile(game.Value));
				DownloadList.Items.Add(new ListItem(
					game.Value.Hash,
					Imaging.CreateBitmapSourceFromHIcon(
						Icon.ExtractAssociatedIcon(
							game.Value.ExecutableFile).Handle,
							Int32Rect.Empty,
							BitmapSizeOptions.FromEmptyOptions()
						),
					game.Value.Detaile.Title
				));
			}
			downloadStart();
		}

		private async void downloadStart() {
			if (DownloadList.Items.Count == 0) {
				return;
			}	
			
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

			var list = DownloadList.Items;

			foreach (var item in DownloadList.Items) {
				var game = item as ListItem;
				if (downloadList[game.Hash].Downloading) {
					continue;
				}

				downloadList[game.Hash].Downloading = true;
				await downloadImage.Run(downloadList[game.Hash].GameDetaile);
				GamesSettings.Instance.Games[game.Hash].DownloadComplete = true;
				DownloadList.Items.Remove(
					new ListItem(
						downloadList[game.Hash].GameDetaile.Hash,
						Imaging.CreateBitmapSourceFromHIcon(
							Icon.ExtractAssociatedIcon(
								downloadList[game.Hash].GameDetaile.ExecutableFile).Handle,
								Int32Rect.Empty,
								BitmapSizeOptions.FromEmptyOptions()
							),
						downloadList[game.Hash].GameDetaile.Detaile.Title
					)
				);
			}
			GamesSettings.Instance.Save();
		}
	}
}
