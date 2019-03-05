using LazuriteUI.Icons;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
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

namespace NotificationUITV
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class NotificationWindowTv : Window
    {
        public static readonly DependencyProperty RowBackgroundProperty;

        static NotificationWindowTv()
        {
            RowBackgroundProperty = DependencyProperty.Register(nameof(RowBackground), typeof(Brush), typeof(NotificationWindowTv));
        }

        private Timer _hideTimer;

        public NotificationWindowTv()
        {
            InitializeComponent();

            Width = System.Windows.SystemParameters.PrimaryScreenWidth;
            Height = System.Windows.SystemParameters.PrimaryScreenHeight;

            RowBackground = Brushes.DarkSlateBlue;

            Loaded += (o, e) =>
            {
                Visibility = Visibility.Collapsed;
                HideAnimate();
            };
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            tb.MaxWidth = sizeInfo.NewSize.Width - 90;
        }

        public void Notify(string text, bool special, SolidColorBrush brush = null, Icon icon = LazuriteUI.Icons.Icon.TvNews)
        {
            iconView.Icon = icon;

            if (_hideTimer != null)
            {
                _hideTimer.Change(Timeout.Infinite, Timeout.Infinite);
                _hideTimer = null;
            }
            tb.Text = text;
            var dueTime = special ? 6000 : 3500;
            ShowAnimate(brush);
            _hideTimer = new Timer(
                (s) =>
                {
                    Dispatcher.BeginInvoke(new Action(() => HideAnimate()));
                    _hideTimer = null;
                },
                null, dueTime, Timeout.Infinite);
        }
        
        private void ShowAnimate(SolidColorBrush brush = null)
        {
            Visibility = Visibility.Visible;
            var animation = new ThicknessAnimation();
            animation.Duration = new Duration(TimeSpan.FromMilliseconds(180));
            animation.To = new Thickness(0, 0, 0, 0);
            mainGrid.BeginAnimation(MarginProperty, animation);
            if (brush != null && !brush.Equals(RowBackground))
            {
                RowBackground = new SolidColorBrush(((RowBackground as SolidColorBrush)?.Color ?? Colors.DarkSlateBlue));

                var brushAnimation = new ColorAnimation();
                brushAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(250));
                brushAnimation.To = brush.Color;
                RowBackground.BeginAnimation(SolidColorBrush.ColorProperty, brushAnimation);
            }
        }

        private void HideAnimate()
        {
            var animation = new ThicknessAnimation();
            animation.Completed += (o,e) => Visibility = Visibility.Collapsed;
            animation.Duration = new Duration(TimeSpan.FromMilliseconds(180));
            animation.To = new Thickness(0, ActualHeight, 0, - ActualHeight);
            mainGrid.BeginAnimation(MarginProperty, animation);
        }

        public Brush RowBackground
        {
            get => GetValue(RowBackgroundProperty) as Brush;
            set => SetValue(RowBackgroundProperty, value);
        }
    }
}
