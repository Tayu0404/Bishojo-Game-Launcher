using BishojoGameLauncher.Properties;
using BishojoGameLauncher.Windows;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Input;

namespace BishojoGameLauncher {
	/// <summary>
	/// MainWindow.xaml の相互作用ロジック
	/// </summary>
	public partial class MainWindow : Window {
        private WindowState state { get; set; }
        private Size normalSize { get; set; }
        private double normalTop { get; set; }
        private double normalLeft { get; set; }

        public MainWindow() {
            InitializeComponent();
            initialize();
        }

        private void initialize() {
            if (!Settings.Instance.IsInitialized) {
                var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                string path = Assembly.GetExecutingAssembly().Location;
                key.SetValue("BishojoGameLauncher", path);
                
                Settings.Instance.IsInitialized = true;
            }

            if (Settings.Instance.IsRunAtComputerStartup == 2) {
                this.Hide();
			}
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

        private void window_Closed(object sender, EventArgs e) {
            NotifyIcon.Dispose();
        }

        private void WindowClose_Click(object sender, RoutedEventArgs e) {
            if (Settings.Instance.IsCloseButtonToMinimize) {
                this.Hide();
            } else {
                this.Close();
            }
        }

        private void WindowMaxmize_Click(object sender, RoutedEventArgs e) {
            switch (this.state) {
                case WindowState.Normal:
                    this.normalSize = new Size(this.Width, this.Height);
                    this.normalTop = this.Top;
                    this.normalLeft = this.Left;
                    this.Width = SystemParameters.WorkArea.Width;
                    this.Height = SystemParameters.WorkArea.Height;
                    this.Top = 0;
                    this.Left = 0;
                    this.state = WindowState.Maximized;
                    WindowMaxmize.Tag = "Maxmize";
                    break;
                case WindowState.Maximized:
                    this.Width = this.normalSize.Width;
                    this.Height = this.normalSize.Height;
                    this.Top = this.normalTop;
                    this.Left = this.normalLeft;
                    this.state = WindowState.Normal;
                    WindowMaxmize.Tag = "Normal";
                    break;
            }
        }

		private void WindowMiniMize_Click(object sender, RoutedEventArgs e) {
            this.WindowState = WindowState.Minimized;
		}


		private void AddGame_Click(object sender, RoutedEventArgs e) {
            var addGameWindow = new AddGameWindow();
            addGameWindow.Owner = GetWindow(this);
            addGameWindow.ShowDialog();
            if ((bool)addGameWindow.DialogResult) {
                GameListWindow.Reload();
                DownloadListWindow.Reload();
            }
		}

        private void ChangeGameListWindow_Click(object sender, RoutedEventArgs e) {
            this.GameListWindow.Visibility = Visibility.Visible;
            this.DownloadListWindow.Visibility = Visibility.Hidden;
            this.SettingWindow.Visibility = Visibility.Hidden;
        }
        
		private void ChangeDownloadListWindow_Click(object sender, RoutedEventArgs e) {
            this.GameListWindow.Visibility = Visibility.Hidden;
            this.DownloadListWindow.Visibility = Visibility.Visible;
            this.SettingWindow.Visibility = Visibility.Hidden;
        }

		private void Setting_Click(object sender, RoutedEventArgs e) {
            this.GameListWindow.Visibility = Visibility.Hidden;
            this.DownloadListWindow.Visibility = Visibility.Hidden;
            this.SettingWindow.Visibility = Visibility.Visible;
        }

		private void AppVersionInfo_Click(object sender, RoutedEventArgs e) {
            var versionInfoWindow = new VersionInfoWindow();
            versionInfoWindow.Owner = GetWindow(this);
            versionInfoWindow.ShowDialog();
        }

		private void FilePath_Drop(object sender, DragEventArgs e) {
            var files = e.Data.GetData(DataFormats.FileDrop) as string[];
            if (!File.Exists(files[0])) {
                return;
            }

            var addGameWindow = new AddGameWindow(files[0]);
            addGameWindow.Owner = GetWindow(this);
            addGameWindow.ShowDialog();
            if ((bool)addGameWindow.DialogResult) {
                GameListWindow.Reload();
                DownloadListWindow.Reload();
            }
        }

		private void FilePath_PreviewDragOver(object sender, DragEventArgs e) {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, true))
                e.Effects = DragDropEffects.Copy;
            else
                e.Effects = DragDropEffects.None;
            e.Handled = true;
        }

		private void TaskBarIcon_Show_Click(object sender, RoutedEventArgs e) {
            if (WindowState == WindowState.Minimized) {
                WindowState = WindowState.Normal;
            } else {
                Show();
            }
            Activate();
        }

		private void TaskBarIcon_Exit_Click(object sender, RoutedEventArgs e) {
            this.Close();
		}
	}
	class ShowWindowCommand : ICommand {
        public void Execute(object parameter) {
            if (Application.Current.MainWindow.WindowState == WindowState.Minimized) {
                Application.Current.MainWindow.WindowState = WindowState.Normal;
            } else {
                Application.Current.MainWindow.Show();
            }
            Application.Current.MainWindow.Activate();
        }

        public bool CanExecute(object parameter) {
            return true;
        }
        public event EventHandler CanExecuteChanged;
    }
}
