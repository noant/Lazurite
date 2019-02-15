using Lazurite.IOC;
using Lazurite.MainDomain;
using Lazurite.Windows.Server;
using System.Linq;
using System.Windows.Controls;

namespace LazuriteUI.Windows.Main.Security
{
    /// <summary>
    /// Логика взаимодействия для UsersListViewExt.xaml
    /// </summary>
    public partial class UsersListViewExtended : Grid
    {
        private UsersRepositoryBase _repository = Singleton.Resolve<UsersRepositoryBase>();
        private LazuriteServer _server = Singleton.Resolve<LazuriteServer>();

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
                        _server.RevokeToken(user.Login);
                    },
                    (args) =>
                    {
                        if (args.Password.Length < 6)
                        {
                            args.Message = "Длина пароля должна быть не менее 6 символов";
                            args.Success = false;
                        }
                        else
                            args.Success = true;
                    },
                    user);
            };
            btEdit.Click += (o, e) => {
                var userId = listView.SelectedUsersIds.First();
                var user = _repository.Users.First(x => x.Id.Equals(userId));
                var prevLogin = user.Login;
                EditUserView.Show(
                    () => 
                    {
                        _repository.Save(user);
                        _server.RevokeToken(prevLogin);
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
