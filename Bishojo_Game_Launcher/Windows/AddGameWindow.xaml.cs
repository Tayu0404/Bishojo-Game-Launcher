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
	/// <summary>
	/// Window2.xaml の相互作用ロジック
	/// </summary>
	public partial class AddGameWindow : Window {
		public AddGameWindow() {
			InitializeComponent();
		}

		private void WindowClose_Click(object sender, RoutedEventArgs e) {
			this.Close();
		}

		private void GameRegistry_Click(object sender, RoutedEventArgs e) {
			Game.Game.Add("サクラ").GetAwaiter().GetResult();
		}
	}
}
