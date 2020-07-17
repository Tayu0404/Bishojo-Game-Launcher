using Bishojo_Game_Launcher.Game;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Bishojo_Game_Launcher.Windows {
	public partial class AddGameWindow : Window {
		static Game.List gameList;
		public AddGameWindow() {
			gameList = new Game.List();
			gameList.Read();
			InitializeComponent();
			SearchMode.ItemsSource = new List<SearchGameMode> {
				new SearchGameMode { Mode = ErogameScape.SearchGameMode.Title, ModeName = Properties.Resources.ErogameScapeSearchModeTitle },
				new SearchGameMode { Mode = ErogameScape.SearchGameMode.Brand, ModeName = Properties.Resources.ErogameScapeSearchModeBrand }
			};
			SearchMode.SelectedIndex = 0;
		}

		private class SearchGameMode {
			public ErogameScape.SearchGameMode Mode { get; set; }
			public string ModeName { get; set; }
		}

		private static List<Game.Game.SearchResult> searchGameList;

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

		private async void SearchWord_KeyDown(object sender, System.Windows.Input.KeyEventArgs e) {
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
				var erogameScape = new ErogameScape();
				erogameScape.UseSocks5Proxy("localhost", 8080);
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
			var erogameScape = new ErogameScape();
			erogameScape.UseSocks5Proxy("localhost", 8080);

			var detaile = await erogameScape.GetGameDetails(
				searchGameList[GameList.SelectedIndex].Detaileurl,
				ErogameScape.GetDetaileMode.ID
			);

			gameList.Add(
				Game.Game.GenerateHash(detaile.Title),
				ExecutableFilePath.Text,
				SaveDataPath.Text,
				detaile
			);
			gameList.Write();
			this.Close();
		}
	}
}
