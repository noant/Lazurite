using Lazurite.IOC;
using Lazurite.MainDomain;
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
    /// Логика взаимодействия для UsersListViewExt.xaml
    /// </summary>
    public partial class UsersListViewExtended : Grid
    {
        private UsersRepositoryBase _repository = Singleton.Resolve<UsersRepositoryBase>();

        public UsersListViewExtended()
        {
            InitializeComponent();
            btEdit.IsEnabled = btChangePass.IsEnabled = false;
            listView.SelectionChanged += (ctrl) => btChangePass.IsEnabled = btEdit.IsEnabled = listView.SelectedUsersIds.Any();
            btChangePass.Click += (o, e) => {
                var userId = listView.SelectedUsersIds.First();
                var user = _repository.Users.First(x => x.Id.Equals(userId));
                EditUserPasswordView.Show(() =>
                    {
                        _repository.Save(user);
                    },
                    (args) =>
                    {
                        if (args.Password.Length < 6)
                        {
                            args.Message = "Длина пароля должна быть не менее 6 символов";
                            args.Success = false;
                        }
                        else args.Success = true;
                    },
                user);
            };
            btEdit.Click += (o, e) => {
                var userId = listView.SelectedUsersIds.First();
                var user = _repository.Users.First(x => x.Id.Equals(userId));
                EditUserView.Show(
                    () => {
                        _repository.Save(user);
                        listView.Refresh(user);
                    },
                    (args) =>
                    {
                        if (_repository.Users.Any(x => x.Login.Equals(args.Login) && !x.Id.Equals(user.Id)))
                        {
                            args.Message = "Пользователь с таким логином уже существует";
                            args.Success = false;
                        }
                        else if (_repository.Users.Any(x => x.Name.Equals(args.Name) && !x.Id.Equals(user.Id)))
                        {
                            args.Message = "Пользователь с таким именем уже существует";
                            args.Success = false;
                        }
                        else
                            args.Success = true;
                    },
                    user);
            };
        }
    }
}
