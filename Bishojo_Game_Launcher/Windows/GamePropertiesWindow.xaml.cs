using AngleSharp;
using BishojoGameLauncher.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BishojoGameLauncher.Windows {
	/// <summary>
	/// GamePropertiesWindow.xaml の相互作用ロジック
	/// </summary>
	public partial class GamePropertiesWindow : Window {
		public GamePropertiesWindow(string hash) {
			InitializeComponent();
			var game = GamesSettings.Instance.Games[hash];
			WindowTitle.Text = makeWindowTitle(game.Detaile.Title);
			HPURL.NavigateUri = new Uri(game.Detaile.Web);
			HPTitle.Text = game.Detaile.Title;
		}

		private string makeWindowTitle(string gameTitle) {
			var maxLength = 15;
			if (gameTitle.Length > maxLength) {
				gameTitle = $"{gameTitle.Substring(0, maxLength)}...";
			}
			var windowTitle = $"{gameTitle} - {Properties.Resources.Properties}";
			return windowTitle;
		}

		private void WindowMiniMize_Click(object sender, RoutedEventArgs e) {
			this.WindowState = WindowState.Minimized;
		}

		private void WindowClose_Click(object sender, RoutedEventArgs e) {
			this.Close();
		}

		private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e) {
			Process.Start(e.Uri.ToString());
		}
	}
}
