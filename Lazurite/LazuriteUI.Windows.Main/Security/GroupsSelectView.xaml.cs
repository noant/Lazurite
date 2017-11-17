using Lazurite.IOC;
using Lazurite.MainDomain;
using LazuriteUI.Windows.Controls;
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
    /// Логика взаимодействия для GroupsSelectView.xaml
    /// </summary>
    public partial class GroupsSelectView : UserControl
    {
        private static UsersRepositoryBase Repository = Singleton.Resolve<UsersRepositoryBase>();

        public GroupsSelectView(string[] selectedGroupsIds)
        {
            InitializeComponent();
            this.groupsView.SelectedGroupsIds = selectedGroupsIds;
        }

        public UserGroupBase[] SelectedGroups
        {
            get
            {
                return this.groupsView.SelectedGroupsIds.Select(x=>Repository.Groups.First(z=>z.Name.Equals(x))).ToArray();
            }
        }

        private void ItemView_Click(object sender, RoutedEventArgs e)
        {
            ApplyClicked?.Invoke();
        }

        public event Action ApplyClicked;

        public static void Show(Action<UserGroupBase[]> callback, string[] selectedGroupsIds)
        {
            var control = new GroupsSelectView(selectedGroupsIds);
            var dialogView = new DialogView(control);
            dialogView.Caption = "Выберите группы";
            control.ApplyClicked += () =>
            {
                callback?.Invoke(control.SelectedGroups);
                dialogView.Close();
            };
            dialogView.Show();
        }
    }
}
