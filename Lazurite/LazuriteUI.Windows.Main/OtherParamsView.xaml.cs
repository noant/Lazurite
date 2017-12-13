using LazuriteUI.Windows.Controls;
using LazuriteUI.Windows.Launcher;
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
                Refresh();
            };
            btRestart.Click += (o, e) => Utils.RestartApp();
            btShutdown.Click += (o, e) => App.Current.Shutdown();
        }

        public void Refresh()
        {
            cbStartOnLogon.Selected = _manager.Settings.RunOnUserLogon;
        }

        public static void Show()
        {
            var paramsView = new OtherParamsView();
            var dialogView = new DialogView(paramsView);
            dialogView.Show();
        }
    }
}
