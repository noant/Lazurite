using AVerCap;
using MediaHost.AverMedia;
using MediaHost.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaHost.Vlc;

namespace MediaHost.LazuritePlugin
{
    public static class MediaObjects
    {
        static MediaObjects() {
            MediaHostWindow = new WPF.MainWindow();
            MediaHostWindow.DataManager = new DataManager();
            MediaHostWindow.Sources = new MediaPanelBase[] {
#if DEBUG
                new TestPanel("Тестовая панель"),
#endif
                new VlcIptvHost("IPTV плеер VLC"),
                new AverMediaHost("Карта захвата AverMedia"),
            };
        }

        public static readonly MediaHost.WPF.MainWindow MediaHostWindow;
    }
}
