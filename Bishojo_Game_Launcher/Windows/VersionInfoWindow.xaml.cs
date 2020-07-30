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
	/// VersionInfoWindow.xaml の相互作用ロジック
	/// </summary>
	public partial class VersionInfoWindow : Window {
		public VersionInfoWindow() {
			InitializeComponent();
			iiitialize();
		}

		private void iiitialize() {
			var version = Assembly.GetExecutingAssembly().GetName().Version;
			AppVersion.Text = version.ToString();

			var copyright = (AssemblyCopyrightAttribute)Attribute.GetCustomAttribute(
				Assembly.GetExecutingAssembly(),
				typeof(AssemblyCopyrightAttribute)
			);
			Copyright.Text = copyright.Copyright;
		}

		private void WindowClose_Click(object sender, RoutedEventArgs e) {
			this.Close();
		}
	}
}
