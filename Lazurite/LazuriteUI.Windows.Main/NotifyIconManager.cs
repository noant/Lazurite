using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazuriteUI.Windows.Main
{
    public static class NotifyIconManager
    {
        static System.Windows.Forms.NotifyIcon _notifyIcon;
        static MainWindow _mainWindow;

        public static void Initialize() {
            _notifyIcon = new System.Windows.Forms.NotifyIcon();
            var iconStream = typeof(Utils)
                    .Assembly
                    .GetManifestResourceStream("LazuriteUI.Windows.Main.Icons.Lazurite32.ico");
            _notifyIcon.Icon = new System.Drawing.Icon(iconStream);
            _notifyIcon.Visible = true;
            _notifyIcon.Text = "Lazurite";
            _notifyIcon.MouseClick += (o, e) => {
                if (e.Button == System.Windows.Forms.MouseButtons.Right)
                {
                    if (_mainWindow == null)
                        Click();
                }
            };
        }

        public static void Click()
        {
            App.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                _mainWindow = new MainWindow();
                _mainWindow.Closing += (o1, e1) => _mainWindow = null;
                _mainWindow.Show();
            }));
        }
    }
}
