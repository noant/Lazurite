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
    /// Логика взаимодействия для GroupsListViewExtended.xaml
    /// </summary>
    public partial class GroupsListViewExtended : Grid
    {
        private UsersRepositoryBase _repository = Singleton.Resolve<UsersRepositoryBase>();

        public GroupsListViewExtended()
        {
            InitializeComponent();
            btUsers.IsEnabled = false;
            listView.SelectionChanged += (ctrl) => btUsers.IsEnabled = listView.SelectedGroups.Any();
            btUsers.Click += (o, e) => {
                var group = listView.SelectedGroups.First();
                UsersSelectView.Show(
                    (users) => {
                        group.Users = users.ToList();
                        _repository.Save(group);
                    },
                    group.Users.ToArray(), 
                    true);
            };
        }

        public static void Show()
        {
            var control = new GroupsListViewExtended();
            var dialogView = new DialogView(control);
            dialogView.Show();
        }
    }
}
