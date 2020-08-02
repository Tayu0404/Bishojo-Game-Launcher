using BishojoGameLauncher.Game;
using BishojoGameLauncher.Properties;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BishojoGameLauncher.Windows {
	public partial class AddGameWindow : Window {
		public AddGameWindow() {
			InitializeComponent();
			SearchMode.ItemsSource = new List<SearchGameMode> {
				new SearchGameMode { Mode = ErogameScape.SearchGameMode.Title, ModeName = Properties.Resources.ErogameScapeSearchModeTitle },
				new SearchGameMode { Mode = ErogameScape.SearchGameMode.Brand, ModeName = Properties.Resources.ErogameScapeSearchModeBrand }
			};
			SearchMode.SelectedIndex = 0;
			InitializeErogameScape();
		}

		private ErogameScape erogameScape;

		private void InitializeErogameScape() {
			erogameScape = new ErogameScape();

			if (Settings.Instance.IsProxyEnable) {

				if (Settings.Instance.ProxyType) {
					/// Socks5
					if (Settings.Instance.ProxyUser != default(string)) {
						erogameScape.UseSocks5Proxy(
							Settings.Instance.ProxyHost,
							Settings.Instance.ProxyPort,
							Settings.Instance.ProxyUser,
							Settings.Instance.ProxyPassword
						);
					} else {
						erogameScape.UseSocks5Proxy(
							Settings.Instance.ProxyHost,
							Settings.Instance.ProxyPort
						);
					}
				} else {
					/// HTTP
					if (Settings.Instance.ProxyUser != default(string)) {
						erogameScape.UseHTTPProxy(
							Settings.Instance.ProxyHost,
							Settings.Instance.ProxyPort,
							Settings.Instance.ProxyUser,
							Settings.Instance.ProxyPassword
						);
					} else {
						erogameScape.UseHTTPProxy(
							Settings.Instance.ProxyHost,
							Settings.Instance.ProxyPort
						);
					}
				}
			}
		}

		private class SearchGameMode {
			public ErogameScape.SearchGameMode Mode { get; set; }
			public string ModeName { get; set; }
		}

		private static List<SearchResult> searchGameList;

		private void WindowClose_Click(object sender, RoutedEventArgs e) {
			this.Close();
		}

		private async void SearchGame_Click(object sender, RoutedEventArgs e) {
			await searchGame();
		}

		private void GameList_SelectionChanged(object sender, RoutedEventArgs e) {
			var listBox = sender as ListBox;
			if (listBox.Items.Count == 0) {
				GameTitle.Text = "";
				GameBrand.Text = "";
				GameReleaseData.Text = "";
				return;
			} else {
				GameTitle.Text = searchGameList[listBox.SelectedIndex].Title;
				GameBrand.Text = searchGameList[listBox.SelectedIndex].Brand;
				GameReleaseData.Text = searchGameList[listBox.SelectedIndex].Sellday;
			}
		}

		private async void SearchWord_KeyDown(object sender, KeyEventArgs e) {
			if (e.Key == Key.Return) {
				await searchGame();
			}
		}

		private async Task searchGame() {
			GameList.Items.Clear();
			if (SearchWord.Text == "") {
				return;
			}
			try {
				var mode = SearchMode.SelectedItem as SearchGameMode;
				searchGameList = await erogameScape.SearchGame(SearchWord.Text, mode.Mode);
				if (searchGameList.Count == 0) {
					GameList.Items.Add(Properties.Resources.NotFound);
				} else {
					foreach (var game in searchGameList) {
						GameList.Items.Add(game.Title);
					}
					GameList.SelectedIndex = 0;
				}
			}
			catch {
				return;
			}
		}

		private void ExecutableFilePath_PreviewDragOver(object sender, DragEventArgs e) {
			if (e.Data.GetDataPresent(DataFormats.FileDrop, true))
				e.Effects = DragDropEffects.Copy;
			else
				e.Effects = DragDropEffects.None;
			e.Handled = true;
		}

		private void ExecutableFilePath_Drop(object sender, DragEventArgs e) {
			var files = e.Data.GetData(DataFormats.FileDrop) as string[];
			if (!File.Exists(files[0])){
				return;
			}
			ExecutableFilePath.Text = files[0];
		}

		private void SaveDataPath_PreviewDragOver(object sender, DragEventArgs e) {
			if (e.Data.GetDataPresent(DataFormats.FileDrop, true))
				e.Effects = DragDropEffects.Copy;
			else
				e.Effects = DragDropEffects.None;
			e.Handled = true;
		}

		private void SaveDataPath_Drop(object sender, DragEventArgs e) {
			var files = e.Data.GetData(DataFormats.FileDrop) as string[];
			if (File.Exists(files[0])) {
				return;
			}
			SaveDataPath.Text = files[0];
		}

		private async void GameRegistry_Click(object sender, RoutedEventArgs e) {

			var detaile = await erogameScape.GetGameDetails(
				searchGameList[GameList.SelectedIndex].Detaileurl,
				ErogameScape.GetDetaileMode.ID
			);
			var hash = Generate.Hash(detaile.Title);

			GamesSettings.Instance.Games.Add(
				hash,
				new GameDetaile(
					hash,
					ExecutableFilePath.Text,
					SaveDataPath.Text,
					detaile,
					false
				)
			);
			GamesSettings.Instance.Save();
			this.Close();
		}

		private void SelectBrowseExecutableFile_Click(object sender, RoutedEventArgs e) {
			var dialog = new CommonOpenFileDialog(Properties.Resources.ExecutableFile);

			dialog.Filters.Add(new CommonFileDialogFilter("EXE File", "*.exe"));
			dialog.Filters.Add(new CommonFileDialogFilter("All File", "*.*"));

			if (dialog.ShowDialog() == CommonFileDialogResult.Ok) {
				var filename = dialog.FileName;
				ExecutableFilePath.Text = filename;
			}
		}

		private void SelectBrowseSaveData_Click(object sender, RoutedEventArgs e) {
			var dialog = new CommonOpenFileDialog(Properties.Resources.SaveDataFolder);

			dialog.IsFolderPicker = true;

			if (dialog.ShowDialog() == CommonFileDialogResult.Ok) {
				var filename = dialog.FileName;
				SaveDataPath.Text = filename;
			}
		}
	}
}
