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
using MediaHost.VlcWrapper.Playlists;

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

            var pl = PlaylistsHelper.FromPath(@"D:\Other\IpTvPlaylist\1.m3u8", (p) => NotificationUtils.ShowNotification(p, NotificationUtils.InfoType.Loading), CancellationToken.None);
            var channels = (pl as Playlist).GetLowNesting();
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

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }
    }
}
