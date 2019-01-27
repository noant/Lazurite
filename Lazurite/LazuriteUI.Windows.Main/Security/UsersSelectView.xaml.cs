using Lazurite.IOC;
using Lazurite.MainDomain;
using LazuriteUI.Windows.Controls;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace LazuriteUI.Windows.Main.Security
{
    /// <summary>
    /// Логика взаимодействия для GroupsSelectView.xaml
    /// </summary>
    public partial class UsersSelectView : UserControl
    {
        private static UsersRepositoryBase Repository = Singleton.Resolve<UsersRepositoryBase>();

        public UsersSelectView(string[] selectedUsersIds, bool hideButtons)
        {
            InitializeComponent();
            usersView.SelectedUsersIds = selectedUsersIds;
            if (hideButtons)
                usersView.HideButtons();
        }

        public UserBase[] SelectedUsers
        {
            get
            {
                return usersView.SelectedUsersIds.Select(x=> Repository.Users.First(z=>z.Id.Equals(x))).ToArray();
            }
        }

        private void ItemView_Click(object sender, RoutedEventArgs e)
        {
            ApplyClicked?.Invoke();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event Action ApplyClicked;

        public static void Show(Action<UserBase[]> callback, string[] selectedUsersIds, bool hideButtons=false)
        {
            if (!Repository.Users.Any())
            {
                MessageView.ShowMessage("Пользователи не созданы!", "Выбор пользователей", Icons.Icon.Warning);
            }
            else
            {
                var control = new UsersSelectView(selectedUsersIds, hideButtons);
                var dialogView = new DialogView(control);
                dialogView.Caption = "Выберите пользователей";
                control.ApplyClicked += () =>
                {
                    callback?.Invoke(control.SelectedUsers);
                    dialogView.Close();
                };
                dialogView.Show();
            }
        }
    }
}
