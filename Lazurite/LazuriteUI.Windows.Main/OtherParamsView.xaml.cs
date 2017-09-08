using LazuriteUI.Windows.Controls;
using LazuriteUI.Windows.Launcher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
