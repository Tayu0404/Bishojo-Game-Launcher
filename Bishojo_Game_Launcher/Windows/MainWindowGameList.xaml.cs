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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Bishojo_Game_Launcher.Windows {
	/// <summary>
	/// MainWindowGameList.xaml の相互作用ロジック
	/// </summary>
	public partial class MainWindowGameList : UserControl {
		private static List<Game.List.GameDetaile> gamelist;

		public MainWindowGameList() {
			InitializeComponent();
			GameListReload();
		}

		public void GameListReload() {
			GameList.Items.Clear();
			var list = new Game.List();
			list.Read();
			gamelist = list.Games;
			Console.WriteLine(gamelist[0].Detaile.Title);
			Console.WriteLine("Test");
			gamelist.Select(x =>
				GameList.Items.Add(x.Detaile.Title)
			);
			GameList.SelectedItem = 0;
		}
	}
}
