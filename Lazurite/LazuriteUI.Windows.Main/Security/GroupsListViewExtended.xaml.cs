using Lazurite.IOC;
using Lazurite.MainDomain;
using LazuriteUI.Windows.Controls;
using System.Linq;
using System.Windows.Controls;

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
            listView.SelectionChanged += (ctrl) => btUsers.IsEnabled = listView.SelectedGroupsIds.Any();
            btUsers.Click += (o, e) => {
                var groupId = listView.SelectedGroupsIds.First();
                var group = _repository.Groups.First(x => x.Name.Equals(groupId));
                UsersSelectView.Show(
                    (users) => {
                        group.UsersIds = users.Select(x=>x.Id).ToList();
                        _repository.Save(group);
                    },
                    group.UsersIds.ToArray(), 
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
