using Lazurite.MainDomain;
using Lazurite.Security;
using Lazurite.Security.Permissions;
using LazuriteUI.Windows.Controls;
using LazuriteUI.Windows.Main.Security.PermissionsViews;
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

namespace LazuriteUI.Windows.Main.Security
{
    /// <summary>
    /// Логика взаимодействия для ScenarioSecurityManagementView.xaml
    /// </summary>
    public partial class ScenarioSecurityManagementView : Grid
    {
        private ScenarioBase _scenario;

        public ScenarioSecurityManagementView(ScenarioBase scenario)
        {
            InitializeComponent();
            _scenario = scenario;

            btAddNew.Click += (o, e) => {
                SelectPermissionView.Show((permission) => {
                    Add(permission);
                    ((SecuritySettings)_scenario.SecuritySettings).Permissions.Add(permission);
                    Modified?.Invoke();
                });
            };

            btClose.Click += (o, e) => CloseClicked?.Invoke();

            Refresh();
        }

        public void Refresh()
        {
            stackPanel.Children.Clear();
            foreach (var permission in ((SecuritySettings)_scenario.SecuritySettings).Permissions)
                Add(permission);
        }

        private void Add(IPermission permission)
        {
            var control = PermissionViewsResolver.CreateControlBy(permission);
            control.Modified += (sender) => Modified?.Invoke();
            control.RemoveClicked += (sender) => {
                ((SecuritySettings)_scenario.SecuritySettings).Permissions.Remove(sender.Permission);
                stackPanel.Children.Remove(control);
                Modified?.Invoke();
            };

            stackPanel.Children.Add(control);
        }
        
        public event Action Modified;
        public event Action CloseClicked;

        public static void Show(ScenarioBase scenario, Action modified)
        {
            var control = new ScenarioSecurityManagementView(scenario);
            var dialog = new DialogView(control);
            control.Modified += () => modified?.Invoke();
            control.CloseClicked += () => dialog.Close();
            dialog.Show();
        }
    }
}
