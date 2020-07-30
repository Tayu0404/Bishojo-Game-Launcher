using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BishojoGameLauncher.Control {
    /// <summary>
    /// ToggleSwitch.xaml の相互作用ロジック
    /// </summary>
    public partial class ToggleSwitch : UserControl {
        public ToggleSwitch() {
            InitializeComponent();
            borderTrack.Background = new SolidColorBrush(Colors.Gray); ;
        }

        private void ButtonToggle_Click(object sender, RoutedEventArgs e) {
            IsOn = !IsOn;
        }

        #region ******************************* TrackBackgroundOnColor
        [Category("Switch")]
        public SolidColorBrush TrackBackgroundOnColor {
            get { return (SolidColorBrush)this.GetValue(TrackBackgroundOnColorProperty); }
            set { this.SetValue(TrackBackgroundOnColorProperty, value); }
        }

        public static readonly DependencyProperty TrackBackgroundOnColorProperty =
            DependencyProperty.Register("TrackBackgroundOnColor", typeof(SolidColorBrush),
                typeof(ToggleSwitch),
                new FrameworkPropertyMetadata(new SolidColorBrush(Colors.DeepSkyBlue),
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    TrackBackgroundOnColorChangeFunc,
                    TrackBackgroundOnColorCoerceFunc));

        static void TrackBackgroundOnColorChangeFunc(DependencyObject target,
            DependencyPropertyChangedEventArgs e) {
            var of = (SolidColorBrush)e.OldValue;
            var nf = (SolidColorBrush)e.NewValue;
            var obj = (ToggleSwitch)target;
        }

        static object TrackBackgroundOnColorCoerceFunc(DependencyObject target, object baseValue) {
            var obj = (ToggleSwitch)target;
            var val = (SolidColorBrush)baseValue;

            return val;
        }
        #endregion

        #region ******************************* TrackBackgroundOffColor
        [Category("Switch")]
        public SolidColorBrush TrackBackgroundOffColor {
            get { return (SolidColorBrush)this.GetValue(TrackBackgroundOffColorProperty); }
            set { this.SetValue(TrackBackgroundOffColorProperty, value); }
        }

        public static readonly DependencyProperty TrackBackgroundOffColorProperty =
            DependencyProperty.Register("TrackBackgroundOffColor", typeof(SolidColorBrush),
                typeof(ToggleSwitch),
                new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Gray),
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    TrackBackgroundOffColorChangeFunc));

        static void TrackBackgroundOffColorChangeFunc(DependencyObject target,
            DependencyPropertyChangedEventArgs e) {
            var of = (SolidColorBrush)e.OldValue;
            var nf = (SolidColorBrush)e.NewValue;
            var obj = (ToggleSwitch)target;
            obj.borderTrack.Background = nf;
        }
        #endregion

        #region ******************************* CircleBackgroundColor
        [Category("Switch")]
        public SolidColorBrush CircleBackgroundColor {
            get { return (SolidColorBrush)this.GetValue(CircleBackgroundColorProperty); }
            set { this.SetValue(CircleBackgroundColorProperty, value); }
        }

        public static readonly DependencyProperty CircleBackgroundColorProperty =
            DependencyProperty.Register("CircleBackgroundColor", typeof(SolidColorBrush),
                typeof(ToggleSwitch),
                new FrameworkPropertyMetadata(new SolidColorBrush(Colors.SteelBlue),
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    CircleBackgroundColorChangeFunc));

        static void CircleBackgroundColorChangeFunc(DependencyObject target,
            DependencyPropertyChangedEventArgs e) {
            var of = (SolidColorBrush)e.OldValue;
            var nf = (SolidColorBrush)e.NewValue;
            var obj = (ToggleSwitch)target;
            obj.ellipseToggle.Fill = nf;
        }
        #endregion

        #region ******************************* CircleBorderColor
        [Category("Switch")]
        public SolidColorBrush CircleBorderColor {
            get { return (SolidColorBrush)this.GetValue(CircleBorderColorProperty); }
            set { this.SetValue(CircleBorderColorProperty, value); }
        }

        public static readonly DependencyProperty CircleBorderColorProperty =
            DependencyProperty.Register("CircleBorderColor", typeof(SolidColorBrush),
                typeof(ToggleSwitch),
                new FrameworkPropertyMetadata(new SolidColorBrush(Colors.White),
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    CircleBorderColorChangeFunc));

        static void CircleBorderColorChangeFunc(DependencyObject target,
            DependencyPropertyChangedEventArgs e) {
            var of = (SolidColorBrush)e.OldValue;
            var nf = (SolidColorBrush)e.NewValue;
            var obj = (ToggleSwitch)target;

            obj.ellipseToggle.Stroke = nf;
        }
        #endregion

        //BindsTwoWayByDefaultを設定する事
        #region ******************************* IsOn
        [Category("Switch")]
        [Description("Switch status")]
        public bool IsOn {
            get { return (bool)this.GetValue(IsOnProperty); }
            set { this.SetValue(IsOnProperty, value); }
        }
        public static readonly DependencyProperty IsOnProperty =
            DependencyProperty.Register("IsOn", typeof(bool),
                typeof(ToggleSwitch),
                new FrameworkPropertyMetadata(false,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    IsOnChangeFunc));

        static void IsOnChangeFunc(DependencyObject target,
            DependencyPropertyChangedEventArgs e) {
            var of = (bool)e.OldValue;
            var nf = (bool)e.NewValue;
            var obj = (ToggleSwitch)target;

            if (nf) {
                obj.borderTrack.Background = new SolidColorBrush(obj.TrackBackgroundOffColor.Color);
                var ca = new ColorAnimation(obj.TrackBackgroundOnColor.Color, TimeSpan.FromSeconds(obj.Duration));
                obj.borderTrack.Background.BeginAnimation(SolidColorBrush.ColorProperty, ca);
                var da = new DoubleAnimation(10, TimeSpan.FromSeconds(obj.Duration));
                obj.translateTransform.BeginAnimation(TranslateTransform.XProperty, da);
            } else {
                obj.borderTrack.Background = new SolidColorBrush(obj.TrackBackgroundOnColor.Color);
                var ca = new ColorAnimation(obj.TrackBackgroundOffColor.Color, TimeSpan.FromSeconds(obj.Duration));
                obj.borderTrack.Background.BeginAnimation(SolidColorBrush.ColorProperty, ca);
                var da = new DoubleAnimation(-10, TimeSpan.FromSeconds(obj.Duration));
                obj.translateTransform.BeginAnimation(TranslateTransform.XProperty, da);
            }
        }
        #endregion

        #region ******************************* Duration
        [Category("Switch")]
        [Description("Change Duration (sec)")]
        public double Duration {
            get { return (double)this.GetValue(DurationProperty); }
            set { this.SetValue(DurationProperty, value); }
        }

        public static readonly DependencyProperty DurationProperty =
            DependencyProperty.Register("ChangeDuration", typeof(double),
                typeof(ToggleSwitch),
                new PropertyMetadata(0.05));
        #endregion

    }
}