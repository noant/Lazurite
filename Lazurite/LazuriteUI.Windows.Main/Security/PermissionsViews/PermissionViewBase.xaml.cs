using Lazurite.Security.Permissions;
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
            Refresh();
        }

        public IPermission Permission { get; private set; }

        public void Refresh()
        {
            tbName.Text = PermissionName;
            btSelect.Visibility = IsSelectButtonVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        public virtual bool IsSelectButtonVisible
        {
            get
            {
                return true;
            }
        }

        public string PermissionName
        {
            get
            {
                return Lazurite.ActionsDomain.Utils.ExtractHumanFriendlyName(Permission.GetType());
            }
        }

        public virtual void OnSelectClick()
        {
            //do nothing
        }

        public void OnModified()
        {
            Modified?.Invoke(this);
        }

        public event Action<PermissionViewBase> RemoveClicked;
        public event Action<PermissionViewBase> Modified;
    }
}
