using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace Bishojo_Game_Launcher {
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window {

        private WindowState state { get; set; }
        private Size normalSize { get; set; }



        public MainWindow() {
            InitializeComponent();
        }

        private void window_StateChanged(object sender, EventArgs e) {
            switch (WindowState) {
                case WindowState.Maximized:
                    LayoutRoot.Margin = new Thickness(9);
                    break;
                default:
                    LayoutRoot.Margin = new Thickness(0);
                    break;
            }
        }
        private void WindowClose_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }

        private void WindowClose_Maxmize(object sender, RoutedEventArgs e) {
            switch (this.state) {
                case WindowState.Normal:
                    normalSize = new Size(this.Width, this.Height);
                    this.Width = SystemParameters.WorkArea.Width;
                    this.Height = SystemParameters.WorkArea.Height;
                    this.Left = 0;
                    this.Top = 0;
                    this.state = WindowState.Maximized;
                    break;
                case WindowState.Maximized:
                    this.Width = normalSize.Width;
                    this.Height = normalSize.Height;
                    this.state = WindowState.Normal;
                    break;
            }
        }

        private void AddGane_Click(object sender, RoutedEventArgs e) {
        }
    }
}
