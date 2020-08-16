using BishojoGameLauncher.Game;
using BishojoGameLauncher.Properties;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static BishojoGameLauncher.Windows.GamePropertiesWindow;

namespace BishojoGameLauncher.Windows {
	/// <summary>
	/// MainWindowGameList.xaml の相互作用ロジック
	/// </summary>
	public partial class MainWindowGameList : UserControl {

		public MainWindowGameList() {
			InitializeComponent();
			initialize();
		}

		private void initialize() {
			var mode = Settings.Instance.GameListSortMode;
			switch (mode) {
				case 0:
				case 1:
					SortModeSelect.SelectedIndex = 0;
					break;
				case 2:
				case 3:
					SortModeSelect.SelectedIndex = 1;
					break;
				case 4:
				case 5:
					SortModeSelect.SelectedIndex = 2;
					break;
				case 6:
				case 7:
					SortModeSelect.SelectedIndex = 3;
					break;
				case 8:
				case 9:
					SortModeSelect.SelectedIndex = 4;
					break;
				case 10:
				case 11:
					SortModeSelect.SelectedIndex = 5;
					break;
			}
			if (mode % 2 != 0) {
				EnableIndex.IsOn = true;
				GameList.SelectedIndex = 1;
			} else {
				GameList.SelectedIndex = 0;
			}
		}

		private SortMode _sortMode;

		private SortMode sortMode {
			get { return _sortMode; }
			set {
				_sortMode = value;
				Reload();
			}
		}

		private class GameListItem {
			public GameListItem(string hash, BitmapSource appIcon, string title, bool enable = true) {
				this.Hash = hash;
				this.AppIcon = appIcon;
				this.Title = title;
				this.Enable = enable;
			}	

			public string Hash { get; private set; }

			public BitmapSource AppIcon { get; set; }

			public string Title { get; private set; }

			public bool Enable { get; private set; }
		}

		private enum SortMode {
			TitleAscendingOrder             = 0,
			TitleAscendingOrderWithIndex    = 1,
			TitleDescendingOrder            = 2,
			TitleDescendingOrderWithIndex   = 3,
			BrandAscendingOrder             = 4,
			BrandAscendingOrderWithIndex    = 5,
			BrandDescendingOrder            = 6,
			BrandDescendingOrderWithIndex   = 7,
			SellDayAscendingOrder           = 8,
			SellDayAscendingOrderWithIndex  = 9,
			SellDayDescendingOrder          = 10,
			SellDayDescendingOrderWithIndex = 11
		}

		public void Reload() { 
			GameList.Items.Clear();
			var games = GamesSettings.Instance.Games;
			IOrderedEnumerable<KeyValuePair<string, GameDetaile>> sortedGames;
			switch (sortMode) {
				case SortMode.TitleAscendingOrder:
				case SortMode.TitleAscendingOrderWithIndex:
					sortedGames = Sort.TitleAscendingOrder(games);
					break;
				case SortMode.TitleDescendingOrder:
				case SortMode.TitleDescendingOrderWithIndex:
					sortedGames = Sort.TitleDescendingOrder(games);
					break;
				case SortMode.BrandAscendingOrder:
				case SortMode.BrandAscendingOrderWithIndex:
					sortedGames = Sort.BrandAscendingOrder(games);
					break;
				case SortMode.BrandDescendingOrder:
				case SortMode.BrandDescendingOrderWithIndex:
					sortedGames = Sort.BrandDescendingOrder(games);
					break;
				case SortMode.SellDayAscendingOrder:
				case SortMode.SellDayAscendingOrderWithIndex:
					sortedGames = Sort.SellDayAscendingOrder(games);
					break;
				case SortMode.SellDayDescendingOrder:
				case SortMode.SellDayDescendingOrderWithIndex:
					sortedGames = Sort.SellDayDescendingOrder(games);
					break;
				default:
					sortedGames = Sort.TitleAscendingOrder(games);
					break;
			}
			
			switch (sortMode) {
				case SortMode.TitleAscendingOrderWithIndex:
				case SortMode.TitleDescendingOrderWithIndex:
					var firstLetterBefore = '\0';
					var numbersAndSymbolsFlag = false;
					var kanjiiFlag = false;
					foreach (var game in sortedGames) {
						var firstLetter = Convert.ToChar(game.Value.Detaile.Title.Substring(0, 1));

						//If it is the first character that has never existed, register it as an index
						//Check if the first letteris Hiragana or Katakana
						if (
							(
								('\u3041' <= firstLetter && firstLetter <= '\u309F') || 
								firstLetter == '\u30FC' || firstLetter == '\u30A0'
							) ||
							(
								('\u30A0' <= firstLetter && firstLetter <= '\u30FF') ||
								('\u31F0' <= firstLetter && firstLetter <= '\u31FF') ||
								('\u3099' <= firstLetter && firstLetter <= '\u309C')
							)
						) {
							firstLetter = Convert.ToChar(Strings.StrConv(firstLetter.ToString(), VbStrConv.Hiragana, 0x411));
							if (firstLetter != firstLetterBefore){
								GameList.Items.Add(
									new GameListItem(
										string.Empty,
										null,
										firstLetter.ToString(),
										false
									)
								);
							}
						} else if (
							//Check if the first letter is in English
							(
								('A' <= firstLetter && firstLetter <= 'Z') ||
								('Ａ' <= firstLetter && firstLetter <= 'Ｚ')
							) ||
							(
								('a' <= firstLetter && firstLetter <= 'z') ||
								('ａ' <= firstLetter && firstLetter <= 'ｚ')
							)
						) {
							if (firstLetter != firstLetterBefore) {
								GameList.Items.Add(
									new GameListItem(
										string.Empty,
										null,
										firstLetter.ToString().ToUpper(),
										false
									)
								);
							}
						} else if (
							//Check if the first letter is Kanji
							('\u4E00' <= firstLetter && firstLetter <= '\u9FCF') ||
							('\uF900' <= firstLetter && firstLetter <= '\uFAFF') ||
							('\u3400' <= firstLetter && firstLetter <= '\u4DBF')
						) {
							if (!kanjiiFlag) {
								GameList.Items.Add(
									new GameListItem(
										string.Empty,
										null,
										Properties.Resources.Kanji,
										false
									)
								);
								kanjiiFlag = true;
							}
						} else if (!numbersAndSymbolsFlag) {
							//Numbers and symbols
							GameList.Items.Add(
								new GameListItem(
									string.Empty,
									null,
									"#",
									false
								)
							);
							numbersAndSymbolsFlag = true;
						}
						firstLetterBefore = firstLetter;

						BitmapSource iconIamge;

						if (
							GamesSettings.Instance.Games[game.Value.Hash].CustomaIconPath != "" &&
							GamesSettings.Instance.Games[game.Value.Hash].CustomaIconPath != null
						) {
							Console.WriteLine("Debug Point");
							iconIamge = new BitmapImage(
								new Uri(GamesSettings.Instance.Games[game.Value.Hash].CustomaIconPath)
							);
						} else {
							iconIamge = Imaging.CreateBitmapSourceFromHIcon(
								Icon.ExtractAssociatedIcon(game.Value.ExecutableFile).Handle,
								Int32Rect.Empty,
								BitmapSizeOptions.FromEmptyOptions()
							);
						}

						GameList.Items.Add(
							new GameListItem(
								game.Value.Hash,
								iconIamge,
								game.Value.Detaile.Title
							)
						);
					}
					break;
				case SortMode.BrandAscendingOrderWithIndex:
				case SortMode.BrandDescendingOrderWithIndex:
					var brandBefore = "";
					foreach (var game in sortedGames) {
						var brand = game.Value.Detaile.Brand;
						if (brand != brandBefore) {
							GameList.Items.Add(
								new GameListItem(
									string.Empty,
									null,
									brand,
									false
								)
							);
						}
						brandBefore = brand;

						BitmapSource iconIamge;

						if (
							GamesSettings.Instance.Games[game.Value.Hash].CustomaIconPath != "" &&
							GamesSettings.Instance.Games[game.Value.Hash].CustomaIconPath != null
						) {
							Console.WriteLine("Debug Point");
							iconIamge = new BitmapImage(
								new Uri(GamesSettings.Instance.Games[game.Value.Hash].CustomaIconPath)
							);
						} else {
							iconIamge = Imaging.CreateBitmapSourceFromHIcon(
								Icon.ExtractAssociatedIcon(game.Value.ExecutableFile).Handle,
								Int32Rect.Empty,
								BitmapSizeOptions.FromEmptyOptions()
							);
						}

						GameList.Items.Add(
							new GameListItem(
								game.Value.Hash,
								iconIamge,
								game.Value.Detaile.Title
							)
						);
					}
					break;
				case SortMode.SellDayAscendingOrderWithIndex:
				case SortMode.SellDayDescendingOrderWithIndex:
					var selldayBefore = "";
					foreach (var game in sortedGames) {
						var sellday = game.Value.Detaile.Sellday.Substring(0,7);
						if (sellday != selldayBefore) {
							GameList.Items.Add(
								new GameListItem(
									string.Empty,
									null,
									sellday,
									false
								)
							);
						}
						selldayBefore = sellday;

						BitmapSource iconIamge;

						if (
							GamesSettings.Instance.Games[game.Value.Hash].CustomaIconPath != "" &&
							GamesSettings.Instance.Games[game.Value.Hash].CustomaIconPath != null
						) {
							Console.WriteLine("Debug Point");
							iconIamge = new BitmapImage(
								new Uri(GamesSettings.Instance.Games[game.Value.Hash].CustomaIconPath)
							);
						} else {
							iconIamge = Imaging.CreateBitmapSourceFromHIcon(
								Icon.ExtractAssociatedIcon(game.Value.ExecutableFile).Handle,
								Int32Rect.Empty,
								BitmapSizeOptions.FromEmptyOptions()
							);
						}

						GameList.Items.Add(
							new GameListItem(
								game.Value.Hash,
								iconIamge,
								game.Value.Detaile.Title
							)
						);
					}
					break;
				default:
					foreach (var game in sortedGames) {
						BitmapSource iconIamge;

						if (
							GamesSettings.Instance.Games[game.Value.Hash].CustomaIconPath != "" &&
							GamesSettings.Instance.Games[game.Value.Hash].CustomaIconPath != null
						) {
							iconIamge = new BitmapImage(
								new Uri(GamesSettings.Instance.Games[game.Value.Hash].CustomaIconPath)
							);
						} else {
							iconIamge = Imaging.CreateBitmapSourceFromHIcon(
								Icon.ExtractAssociatedIcon(game.Value.ExecutableFile).Handle,
								Int32Rect.Empty,
								BitmapSizeOptions.FromEmptyOptions()
							);
						}

						GameList.Items.Add(
							new GameListItem(
								game.Value.Hash,
								iconIamge,
								game.Value.Detaile.Title
							)
						);
					}
					break;
			}
		}

		private void GameList_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			var listBox = sender as ListBox;
			if (listBox.SelectedIndex == -1) {
				return;
			}

			Title.Text = "";
			Brand.Text = "";
			ReleseData.Text = "";
			MainImage.Source = null;
			Illustrator.Items.Clear();
			Scenarios.Items.Clear();
			Composer.Items.Clear();
			VoiceActor.Items.Clear();
			Singer.Items.Clear();
			SelectedGameDetails.ScrollToTop();

			var selectedItem = listBox.Items[listBox.SelectedIndex] as GameListItem;
			var selectedGameDetaile = GamesSettings.Instance.Games[selectedItem.Hash];

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

			if (selectedGameDetaile.DownloadComplete) {
				MainImage.Source = new BitmapImage(
					new Uri(
						AppPath.GamesFolder +
						selectedGameDetaile.Hash + @"\" +
						selectedGameDetaile.Hash + Path.GetExtension(selectedGameDetaile.Detaile.MainImage)
					)
				);
			}
		}

		private void SearchIcon_Click(object sender, RoutedEventArgs e) {
			GameSearchWord.Focus();
		}

		private void Play_Click(object sender, RoutedEventArgs e) {
			var selectedGameDetaile = GameList.SelectedItem as GameListItem;
			var processInfo = new ProcessStartInfo();
			processInfo.FileName = GamesSettings.Instance.Games[selectedGameDetaile.Hash].ExecutableFile;
			if (GamesSettings.Instance.Games[selectedGameDetaile.Hash].StartOptions != ""){
				processInfo.Arguments = GamesSettings.Instance.Games[selectedGameDetaile.Hash].StartOptions;
			}
			Process.Start(processInfo);
		}

		private void Delete_Click(object sender, RoutedEventArgs e) {
			var selectedIndex = GameList.SelectedIndex;
			var selectedGameDetaile = GameList.SelectedItem as GameListItem;
			GamesSettings.Instance.Games.Remove(selectedGameDetaile.Hash);
			GamesSettings.Instance.Save();
			Reload();
			if (GameList.Items.Count == 0) {
				return;
			}
			GameListItem itemToBeSelected;

			switch (sortMode) {
				case SortMode.TitleAscendingOrderWithIndex:
				case SortMode.TitleDescendingOrderWithIndex:
				case SortMode.BrandAscendingOrderWithIndex:
				case SortMode.BrandDescendingOrderWithIndex:
				case SortMode.SellDayAscendingOrderWithIndex:
				case SortMode.SellDayDescendingOrderWithIndex:
					if (selectedIndex == 1) {
						GameList.SelectedIndex = 1;
						return;
					}
					var index = selectedIndex - 2;
					var item = GameList.Items[index];
					itemToBeSelected = item as GameListItem;
					break;
				default:
					if (selectedIndex == 0) {
						GameList.SelectedIndex = 0;
						return;
					}
					index = selectedIndex - 1;
					item = GameList.Items[index];
					itemToBeSelected = item as GameListItem;
					break;

			}

			if (selectedIndex >= GameList.Items.Count) {
				GameList.SelectedIndex = GameList.Items.Count - 1;
			} else if (itemToBeSelected.Enable) {
				GameList.SelectedIndex = selectedIndex - 2;
			} else {
				GameList.SelectedIndex = selectedIndex - 1;
			}
		}

		private void SortMode_Changed(object sender, EventArgs e) {
			var selectedItem= SortModeSelect.SelectedItem as ComboBoxItem;
			if (int.TryParse(selectedItem.Tag.ToString(), out int tag)) {
				var mode = tag;
				if (EnableIndex.IsOn) {
					mode++;
				}
				if (mode == Settings.Instance.GameListSortMode) {
					SetToDefaultSortMode.IsEnabled = false;
				} else {
					SetToDefaultSortMode.IsEnabled = true;
				}

				sortMode = (SortMode)Enum.ToObject(typeof(SortMode), mode);
			}
		}

		private void SetToDefaultSortMode_Click(object sender, RoutedEventArgs e) {
			Settings.Instance.GameListSortMode = (int)sortMode;
			Settings.Instance.Save();
			SetToDefaultSortMode.IsEnabled = false;
		}

		private void GameProperties_Click(object sender, RoutedEventArgs e) {
			var selectedGame = GameList.SelectedItem as GameListItem;
			var gamePropertiesWindow = new GamePropertiesWindow(selectedGame.Hash);
			gamePropertiesWindow.ChangeIcon += changeIcon;
			gamePropertiesWindow.DeleteGame += deleteGame;
			gamePropertiesWindow.Owner = Window.GetWindow(this);
			gamePropertiesWindow.Show();
		}

		private void changeIcon(object sender, ChangeIconEventArgs e) {
			var selectedIndex = GameList.SelectedIndex;
			Reload();
			GameList.SelectedIndex = selectedIndex;
		/*
		 * 処理負荷削減のテスト
			var selectedItem = GameList.SelectedItem as GameListItem;
			selectedItem.AppIcon  = new BitmapImage(new Uri(e.ChangeIconPath));
			GameList.SelectedItem = selectedItem;
		*/
		}

		private void deleteGame(object sender, EventArgs e) {
			var selectedIndex = GameList.SelectedIndex;
			Reload();
			if (GameList.Items.Count == 0) {
				return;
			}
			GameListItem itemToBeSelected;
			var mode = (int)sortMode;
			if (mode % 2 != 0) {
				if (selectedIndex == 1) {
					GameList.SelectedIndex = 1;
					return;
				}
				var index = selectedIndex - 2;
				var item = GameList.Items[index];
				itemToBeSelected = item as GameListItem;
			} else {
				if (selectedIndex == 0) {
					GameList.SelectedIndex = 0;
					return;
				}
				var index = selectedIndex - 1;
				var item = GameList.Items[index];
				itemToBeSelected = item as GameListItem;
			}

			if (selectedIndex >= GameList.Items.Count) {
				GameList.SelectedIndex = GameList.Items.Count - 1;
			} else if (!itemToBeSelected.Enable) {
				GameList.SelectedIndex = selectedIndex - 2;
			} else {
				GameList.SelectedIndex = selectedIndex - 1;
			}
		}
	}
}
