using BishojoGameLauncher.Properties;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

namespace BishojoGameLauncher.Windows {
	/// <summary>
	/// Window1.xaml の相互作用ロジック
	/// </summary>
	public partial class MainWindowSetting : UserControl {
		public MainWindowSetting() {
			InitializeComponent();
			initialize();
		}

		private void initialize() {
			General_RunAtComputerStartup.IsOn = Settings.Instance.IsRunAtComputerStartup;
			General_ShowIconInTray.IsOn = Settings.Instance.IsShowIconInTray;
			NetWork_ProxyEnable.IsOn = Settings.Instance.IsProxyEnable;
			NetWork_ProxyHost.Text = Settings.Instance.ProxyHost;
		}

		private void NetWork_ProxyEnable_Change(object sender, EventArgs e) {
			Settings.Instance.IsProxyEnable = NetWork_ProxyEnable.IsOn;
			Settings.Instance.Save();
		}

		private void General_RunAtComputerStartup_Change(object sender, EventArgs e) {
			Settings.Instance.IsRunAtComputerStartup = General_RunAtComputerStartup.IsOn;
			Settings.Instance.Save();

			var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
			if (General_RunAtComputerStartup.IsOn) {
				string path = Assembly.GetExecutingAssembly().Location;
				key.SetValue("BishojoGameLauncher", path);
			} else {
				key.DeleteValue("BishojoGameLauncher");
			}
		}

		private void General_ShowIconInTray_Change(object sender, EventArgs e) {
			Settings.Instance.IsShowIconInTray = General_ShowIconInTray.IsOn;
			Settings.Instance.Save();
		}
	}
}
