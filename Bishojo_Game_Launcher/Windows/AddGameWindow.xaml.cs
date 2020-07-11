using Bishojo_Game_Launcher.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Bishojo_Game_Launcher.Windows {
	public partial class AddGameWindow : Window {
		public AddGameWindow() {
			InitializeComponent();
		}

		private static List<Dictionary<string, string>> gameList;

		private void WindowClose_Click(object sender, RoutedEventArgs e) {
			this.Close();
		}

		private async void SearchGame_Click(object sender, RoutedEventArgs e) {
			GameList.Items.Clear();
			if (SearchWord.Text == "") {
				return;
			}
			try {
				gameList = await Game.Game.Search(SearchWord.Text);
				if (gameList.Count == 0) {
					GameList.Items.Add(Properties.Resources.NotFound);
				} else {
					foreach (var game in gameList) {
						GameList.Items.Add(game["Title"]);
					}
					GameList.SelectedIndex = 0;
				}
			}
			catch {
				return;
			}
		}

		private void GameList_SelectionChanged(object sender, RoutedEventArgs e) {
			var listBox = sender as ListBox;
			if (listBox.Items.Count == 0) {
				GameTitle.Text = "";
				GameBrand.Text = "";
				GameReleaseData.Text = "";
				return;
			} else {
				GameTitle.Text = gameList[listBox.SelectedIndex]["Title"];
				GameBrand.Text = gameList[listBox.SelectedIndex]["Brand"];
				GameReleaseData.Text = gameList[listBox.SelectedIndex]["Sellday"];
			}
		}
	}
}
