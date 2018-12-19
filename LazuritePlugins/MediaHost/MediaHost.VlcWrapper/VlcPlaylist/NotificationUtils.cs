using LazuriteUI.Icons;
using NotificationUITV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MediaHost.VlcWrapper.Playlists
{
    public static class NotificationUtils
    {
        private static NotificationWindowTv NotificationWindow;
        
        public static void Initialize()
        {
            NotificationWindow = new NotificationWindowTv();
        }

        public static void ShowNotification(string text, InfoType type)
        {
            NotificationWindow.Dispatcher.BeginInvoke(new Action(() => {
                var brush = Brushes[type];
                var icon = Icons[type];
                NotificationWindow.Notify(text, false, brush, icon);
            }));
        }

        public enum InfoType
        {
            Error,
            ChannelOK,
            Loading,
            VlcLoading
        }

        private static readonly Dictionary<InfoType, SolidColorBrush> Brushes = new Dictionary<InfoType, SolidColorBrush>() {
            { InfoType.ChannelOK, new SolidColorBrush(Colors.DodgerBlue) },
            { InfoType.Error, new SolidColorBrush(Colors.Crimson) },
            { InfoType.Loading, new SolidColorBrush(Colors.DarkSlateBlue) },
            { InfoType.VlcLoading, new SolidColorBrush(Colors.Orange)  }
        };

        private static readonly Dictionary<InfoType, Icon> Icons = new Dictionary<InfoType, Icon>() {
            { InfoType.ChannelOK, Icon.TvNews },
            { InfoType.Error, Icon.Noentry },
            { InfoType.Loading, Icon.Hourglass },
            { InfoType.VlcLoading, Icon.Hourglass }
        };
    }
}
