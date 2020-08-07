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
			sortMode = (SortMode)Enum.ToObject(typeof(SortMode), Settings.Instance.GameListSortMode);
			GameList.SelectedIndex = 0;
			var mode = Settings.Instance.GameListSortMode;
			if (mode % 2 != 0) {
				mode--;
				EnableIndex.IsOn = true;
			}
			switch(mode) {
				case 0:
					SortModeSelect.SelectedIndex = 0;
					break;
				case 2:
					SortModeSelect.SelectedIndex = 1;
					break;
				case 4:
					SortModeSelect.SelectedIndex = 2;
					break;
				case 6:
					SortModeSelect.SelectedIndex = 3;
					break;
				case 8:
					SortModeSelect.SelectedIndex = 4;
					break;
				case 10:
					SortModeSelect.SelectedIndex = 5;
					break;
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

		private class ListItem {
			public ListItem(string hash, BitmapSource appIcon, string title, bool enable = true) {
				this.Hash = hash;
				this.AppIcon = appIcon;
				this.Title = title;
				this.Enable = enable;
			}	

			public string Hash { get; private set; }

			public BitmapSource AppIcon { get; private set; }

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
									new ListItem(
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
									new ListItem(
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
									new ListItem(
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
								new ListItem(
									string.Empty,
									null,
									"#",
									false
								)
							);
							numbersAndSymbolsFlag = true;
						}
						firstLetterBefore = firstLetter;

						GameList.Items.Add(
							new ListItem(
								game.Value.Hash,
								Imaging.CreateBitmapSourceFromHIcon(
									Icon.ExtractAssociatedIcon(game.Value.ExecutableFile).Handle,
									Int32Rect.Empty,
									BitmapSizeOptions.FromEmptyOptions()
								),
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
								new ListItem(
									string.Empty,
									null,
									brand,
									false
								)
							);
						}
						brandBefore = brand;

						GameList.Items.Add(
							new ListItem(
								game.Value.Hash,
								Imaging.CreateBitmapSourceFromHIcon(
									Icon.ExtractAssociatedIcon(game.Value.ExecutableFile).Handle,
									Int32Rect.Empty,
									BitmapSizeOptions.FromEmptyOptions()
								),
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
								new ListItem(
									string.Empty,
									null,
									sellday,
									false
								)
							);
						}
						selldayBefore = sellday;

						GameList.Items.Add(
							new ListItem(
								game.Value.Hash,
								Imaging.CreateBitmapSourceFromHIcon(
									Icon.ExtractAssociatedIcon(game.Value.ExecutableFile).Handle,
									Int32Rect.Empty,
									BitmapSizeOptions.FromEmptyOptions()
								),
								game.Value.Detaile.Title
							)
						);
					}
					break;
				default:
					foreach (var game in sortedGames) {
						GameList.Items.Add(
							new ListItem(
								game.Value.Hash,
								Imaging.CreateBitmapSourceFromHIcon(
									Icon.ExtractAssociatedIcon(game.Value.ExecutableFile).Handle,
									Int32Rect.Empty,
									BitmapSizeOptions.FromEmptyOptions()
								),
								game.Value.Detaile.Title
							)
						);
					}
					break;
			}
		}

		public void Add(string hash, string executableFile, string title ) {
			GameList.Items.Add(
				new ListItem(
					hash,
					Imaging.CreateBitmapSourceFromHIcon(
						Icon.ExtractAssociatedIcon(executableFile).Handle,
						Int32Rect.Empty,
						BitmapSizeOptions.FromEmptyOptions()
					),
					title
				)
			);
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

			var selectedItem = listBox.Items[listBox.SelectedIndex] as ListItem;
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
			var selectedGameDetaile = GameList.SelectedItem as ListItem;
			var processInfo = new ProcessStartInfo();
			processInfo.FileName = GamesSettings.Instance.Games[selectedGameDetaile.Hash].ExecutableFile;
			Process.Start(processInfo);
		}

		private void Delete_Click(object sender, RoutedEventArgs e) {
			var selectedIndex = GameList.SelectedIndex;
			var selectedGameDetaile = GameList.SelectedItem as ListItem;
			GamesSettings.Instance.Games.Remove(selectedGameDetaile.Hash);
			GamesSettings.Instance.Save();
			Reload();
			if (GameList.Items.Count == 0) {
				return;
			}
			GameList.SelectedIndex = selectedIndex - 1;
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
	}
}
