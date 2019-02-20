using System;

namespace LazuriteUI.Windows.Main
{
    public static class NotifyIconManager
    {
        private static System.Windows.Forms.NotifyIcon _notifyIcon;
        private static MainWindow _mainWindow;
        private static FastSwitchWindow _fastWindow;

        public static bool IsFastSwitchWindowActive => _fastWindow != null;

        public static void Initialize()
        {
            _notifyIcon = new System.Windows.Forms.NotifyIcon();
            var iconStream = typeof(Utils)
                    .Assembly
                    .GetManifestResourceStream("LazuriteUI.Windows.Main.Icons.Lazurite32.ico");
            _notifyIcon.Icon = new System.Drawing.Icon(iconStream);
            _notifyIcon.Visible = true;
            _notifyIcon.Text = "Lazurite";
            _notifyIcon.MouseClick += (o, e) =>
            {
                if (e.Button == System.Windows.Forms.MouseButtons.Right)
                {
                    ShowMainWindow();
                }
                else if (e.Button == System.Windows.Forms.MouseButtons.Left)
                {
                    ShowFastSwitchWindow();
                }
            };
        }

        public static void ShowMainWindow()
        {
            App.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (_mainWindow == null)
                {
                    _mainWindow = new MainWindow();
                    _mainWindow.Closing += (o1, e1) => _mainWindow = null;
                    _mainWindow.Show();
                }
                else
                {
                    _mainWindow.WindowState = System.Windows.WindowState.Maximized;
                    _mainWindow.Activate();
                }
            }));
        }

        public static void ShowFastSwitchWindow()
        {
            App.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (_fastWindow == null)
                {
                    _fastWindow = new FastSwitchWindow();
                    _fastWindow.Closing += (o1, e1) => _fastWindow = null;
                    _fastWindow.Show();
                }
                else
                {
                    _fastWindow.Activate();
                }
            }));
        }
    }
}