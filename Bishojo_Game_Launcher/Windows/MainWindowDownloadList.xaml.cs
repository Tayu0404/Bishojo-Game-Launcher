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
	/// MainWindowDownloadList.xaml の相互作用ロジック
	/// </summary>
	public partial class MainWindowDownloadList : UserControl {
		public MainWindowDownloadList() {
			InitializeComponent();
			var test = new { AppIcon = "", Title = "Test", Brand = "00A0", ReleaseData = "2020-07-14" };
			DownloadList.Items.Add(test);
		}
	}
}
