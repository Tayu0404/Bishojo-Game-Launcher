using BishojoGameLauncher.Properties;
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

namespace BishojoGameLauncher.Windows {
	/// <summary>
	/// UserControl1.xaml の相互作用ロジック
	/// </summary>
	public partial class MainWindowSettingNetwork : UserControl {
		public MainWindowSettingNetwork() {
			InitializeComponent();
			initialize();
		}

		private void initialize() {
			ProxyEnable.IsOn = Settings.Instance.IsProxyEnable;
			Host.Text = Settings.Instance.ProxyHost;
		}

		private void ProxyEnable_Change(object sender, EventArgs e) {
			Settings.Instance.IsProxyEnable = ProxyEnable.IsOn;
			Settings.Instance.Save();
		}
	}
}
