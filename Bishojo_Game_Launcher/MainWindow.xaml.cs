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

namespace WindowChromeSample
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void window_StateChanged(object sender, EventArgs e)
        {
            switch (WindowState)
            {
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
            this.WindowState = WindowState.Maximized;
        }

        private void AddGane_Click(object sender, RoutedEventArgs e) {
        }
    }
}
