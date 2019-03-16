using MediaHost.VlcWrapper.Playlists;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace MediaHost.VlcWrapper
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            NotificationUtils.Initialize();

            var pl = PlaylistsHelper.FromPath(@"https://smarttvnews.ru/apps/iptvchannels.m3u", (p) => NotificationUtils.ShowNotification(p, NotificationUtils.InfoType.Loading), CancellationToken.None);
            var channels = (pl as Playlist).Expand();
            vlc.Initialize(channels);
            vlc.Start();
            KeyDown += MainWindow_KeyDown;
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.PageDown)
                vlc.AspectRatio = VlcIptvControl.AspectRatios[0];
            else if (e.Key == Key.PageUp)
                vlc.AspectRatio = VlcIptvControl.AspectRatios[1];
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //
        }
    }
}