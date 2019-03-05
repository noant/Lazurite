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

namespace NotificationUI
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class NotificationWindow : Window
    {
        private static SoundPlayer SoundPlayer = new SoundPlayer(Properties.Resources.sound);
        private static SoundPlayer SoundPlayerDisallow = new SoundPlayer(Properties.Resources.PremiumBeat_0013_cursor_selection_07);
        private static SoundPlayer SoundPlayerAccept = new SoundPlayer(Properties.Resources.PremiumBeat_0013_cursor_selection_15);

        private Timer _hideTimer;

        public static void SoundNotify() => SoundPlayer.Play();
        public static void SoundNotifyDisallow() => SoundPlayerDisallow.Play();
        public static void SoundNotifyAccept() => SoundPlayerAccept.Play();

        public NotificationWindow()
        {
            InitializeComponent();

            Width = System.Windows.SystemParameters.PrimaryScreenWidth;
            Height = System.Windows.SystemParameters.PrimaryScreenHeight;

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

        public void Notify(string text, bool special)
        {
            if (_hideTimer != null)
            {
                _hideTimer.Change(Timeout.Infinite, Timeout.Infinite);
                _hideTimer = null;
            }
            tb.Text = text;
            var dueTime = special ? 6000 : 3500;
            ShowAnimate();
            _hideTimer = new Timer(
                (s) =>
                {
                    Dispatcher.BeginInvoke(new Action(() => HideAnimate()));
                    _hideTimer = null;
                },
                null, dueTime, Timeout.Infinite);
        }
        
        private void ShowAnimate()
        {
            Visibility = Visibility.Visible;
            var animation = new ThicknessAnimation();
            animation.Duration = new Duration(TimeSpan.FromMilliseconds(300));
            animation.To = new Thickness(0);
            mainGrid.BeginAnimation(MarginProperty, animation);
        }

        private void HideAnimate()
        {
            var animation = new ThicknessAnimation();
            animation.Completed += (o,e) => Visibility = Visibility.Collapsed;
            animation.Duration = new Duration(TimeSpan.FromMilliseconds(300));
            animation.To = new Thickness(0, -ActualHeight, 0, ActualHeight);
            mainGrid.BeginAnimation(MarginProperty, animation);
        }
    }
}
