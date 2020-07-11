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

		private void WindowClose_Click(object sender, RoutedEventArgs e) {
			this.Close();
		}

		private async void GameRegistry_Click(object sender, RoutedEventArgs e) {
			try {
				var result = await Game.Game.Search("サクラ");
				if (result.Count == 1) {
					
				} else {
					
				}
			} catch {
				return;
			}
		}

		private async void GameDetaileRegistry_Click(object sender, RoutedEventArgs e) {
			var result = await Game.Game.Search(GameTitle.Text);
			var selectGameSearchResultsWindow = new SearchGameWindow(result);
			selectGameSearchResultsWindow.Show();
		}
	}
}
