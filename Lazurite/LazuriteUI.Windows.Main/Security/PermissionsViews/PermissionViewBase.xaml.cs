using Lazurite.Security.Permissions;
using System;
using System.Windows;
using System.Windows.Controls;

namespace LazuriteUI.Windows.Main.Security.PermissionsViews
{
    /// <summary>
    /// Логика взаимодействия для PermissionViewBase.xaml
    /// </summary>
    public partial class PermissionViewBase : UserControl
    {
        public PermissionViewBase(IPermission permission)
        {
            InitializeComponent();
            Permission = permission;
            btRemove.Click += (o, e) => RemoveClicked?.Invoke(this);
            btSelect.Click += (o, e) => OnSelectClick();
            btAllowRead.Click += (o, e) => ChangeReadAllowed();
            Refresh();
        }

        public IPermission Permission { get; private set; }

        public void Refresh()
        {
            tbName.Text = PermissionName;
            btAllowRead.Selected = Permission.DenyAction == Lazurite.MainDomain.ScenarioAction.Execute;
            btSelect.Visibility = IsSelectButtonVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        public virtual bool IsSelectButtonVisible => true;

        public string PermissionName
        {
            get => Lazurite.ActionsDomain.Utils.ExtractHumanFriendlyName(Permission.GetType());
        }

        public void ChangeReadAllowed()
        {
            if (Permission.DenyAction == Lazurite.MainDomain.ScenarioAction.Execute)
                Permission.DenyAction = Lazurite.MainDomain.ScenarioAction.ViewValue;
            else Permission.DenyAction = Lazurite.MainDomain.ScenarioAction.Execute;
            OnModified();
        }

        public virtual void OnSelectClick()
        {
            //do nothing
        }

        public void OnModified()
        {
            Modified?.Invoke(this);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event Action<PermissionViewBase> RemoveClicked;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event Action<PermissionViewBase> Modified;
    }
}
