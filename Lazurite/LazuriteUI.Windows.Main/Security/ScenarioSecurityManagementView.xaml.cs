using Lazurite.MainDomain;
using Lazurite.Security;
using Lazurite.Security.Permissions;
using LazuriteUI.Windows.Controls;
using LazuriteUI.Windows.Main.Security.PermissionsViews;
using System;
using System.Windows.Controls;

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

#pragma warning disable 67
        public event Action Modified;
        public event Action CloseClicked;
#pragma warning restore 67

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
