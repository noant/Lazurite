using LazuriteUI.Windows.Controls;
using LazuriteUI.Windows.Launcher;
using System.Diagnostics;
using System.Windows.Controls;

namespace LazuriteUI.Windows.Main
{
    /// <summary>
    /// Логика взаимодействия для OtherParams.xaml
    /// </summary>
    public partial class OtherParamsView : UserControl
    {
        private LauncherSettingsManager _manager = new LauncherSettingsManager();

        public OtherParamsView()
        {
            InitializeComponent();
            Refresh();
            cbStartOnLogon.SelectionChanged += (o, e) =>
            {
                _manager.Settings.RunOnUserLogon = cbStartOnLogon.Selected;
                _manager.SaveSettings();
            };

            cbRightSideHover.SelectionChanged += (o, e) => {
                UISettings.Current.MouseRightSideHoverEvent = cbRightSideHover.Selected;
                UISettings.Save();
                RightSideHoverForm.Initialize();
            };

            btRestart.Click += (o, e) => Utils.RestartApp();
            btShutdown.Click += (o, e) => Process.GetCurrentProcess().Kill();
        }

        public void Refresh()
        {
            cbStartOnLogon.Selected = _manager.Settings.RunOnUserLogon;
            cbRightSideHover.Selected = UISettings.Current.MouseRightSideHoverEvent;
        }

        public static void Show()
        {
            var paramsView = new OtherParamsView();
            var dialogView = new DialogView(paramsView);
            dialogView.Show();
        }
    }
}
