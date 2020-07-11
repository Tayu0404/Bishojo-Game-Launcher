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
	/// <summary>
	/// SelectGameSearchResultsWindow.xaml の相互作用ロジック
	/// </summary>
	public partial class SearchGameWindow: Window {
		static List<Dictionary<string, string>> gameList;
		public SearchGameWindow(List<Dictionary<string, string>> resultList) {
			gameList = resultList;
			InitializeComponent();
			foreach (var game in gameList) {
				GameList.Items.Add(game["Title"]);
			}
			GameList.SelectedIndex = 0;
		}

		private void WindowClose_Click(object sender, RoutedEventArgs e) {
			this.Close();
		}

		private void GameList_SelectionChanged(object sender, RoutedEventArgs e) {
			var listBox = sender as ListBox;
			SelectGameBrand.Text = gameList[listBox.SelectedIndex]["Brand"];
			SelectGameReleaseData.Text = gameList[listBox.SelectedIndex]["Sellday"];
		}
	}
}
