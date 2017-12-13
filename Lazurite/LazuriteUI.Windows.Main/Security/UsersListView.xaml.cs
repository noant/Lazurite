using Lazurite.IOC;
using Lazurite.MainDomain;
using Lazurite.Security;
using LazuriteUI.Windows.Controls;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace LazuriteUI.Windows.Main.Security
{
    /// <summary>
    /// Логика взаимодействия для UsersListView.xaml
    /// </summary>
    public partial class UsersListView : Grid
    {
        private UsersRepositoryBase _repository = Singleton.Resolve<UsersRepositoryBase>();

        public UsersListView()
        {
            InitializeComponent();

            Refresh();

            this.itemsView.SelectionChanged += (o, e) => this.SelectionChanged?.Invoke(this);

            btAdd.Click += (o, e) => {
                var user = new User();
                EditUserView.Show(
                    () => {
                        EditUserPasswordView.Show(() =>
                        {
                            _repository.Add(user);
                            AddInternal(user);
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
                    },
                    (args) => 
                    {
                        if (_repository.Users.Any(x => x.Login.Equals(args.Login)))
                        {
                            args.Message = "Пользователь с таким логином уже существует";
                            args.Success = false;
                        }
                        else if (_repository.Users.Any(x => x.Name.Equals(args.Name)))
                        {
                            args.Message = "Пользователь с таким именем уже существует";
                            args.Success = false;
                        }
                        else
                            args.Success = true;
                    },
                    user);
            };

            btRemove.Click += (o, e) => {
                MessageView.ShowYesNo("Вы уверены что хотите удалить выбранных пользователей?", "Удаление пользователей", Icons.Icon.UserDelete,
                    (result) => {
                        if (result)
                        {
                            var selectedUsers = SelectedUsersIds;
                            foreach (var user in selectedUsers)
                                Remove(_repository.Users.First(x=>x.Id.Equals(user)));
                        }
                    });
            };

            this.SelectionChanged += (ctrl) => btRemove.IsEnabled = SelectedUsersIds.Any();
        }

        public void HideButtons()
        {
            btRemove.Visibility = btAdd.Visibility = Visibility.Collapsed;
        }

        public void Refresh()
        {
            itemsView.Children.Clear();
            var selectedUsers = SelectedUsersIds;

            foreach (var user in _repository.Users)
                AddInternal(user);

            SelectedUsersIds = selectedUsers;

            btRemove.IsEnabled = selectedUsers.Any();
        }

        public void Add(UserBase user)
        {
            AddInternal(user).Selected = true;
        }

        public void Refresh(UserBase user)
        {
            var itemView = itemsView.Children.Cast<ItemView>().Single(x => ((UserBase)x.Tag).Id.Equals(user.Id));
            RefreshInternal(itemView, user);
        }

        public void Remove(UserBase user)
        {
            var itemView = itemsView.Children.Cast<ItemView>().Single(x => ((UserBase)x.Tag).Id.Equals(user.Id));
            itemsView.Children.Remove(itemView);
            _repository.Remove(user);
        }

        private ItemView AddInternal(UserBase user)
        {
            var itemView = new ItemView();
            RefreshInternal(itemView, user);
            itemsView.Children.Add(itemView);
            return itemView;
        }

        private void RefreshInternal(ItemView itemView, UserBase user)
        {
            itemView.Icon = Icons.Icon.ChevronRight;
            itemView.Content = user.ToString();
            itemView.Margin = new Thickness(0, 1, 0, 0);
            itemView.Tag = user;
        }

        public bool Multiselect
        {
            get
            {
                return itemsView.SelectionMode == Controls.ListViewItemsSelectionMode.Multiple;
            }
            set
            {
                if (value)
                    itemsView.SelectionMode = Controls.ListViewItemsSelectionMode.Multiple;
                else
                    itemsView.SelectionMode = Controls.ListViewItemsSelectionMode.Single;
            }
        }

        public string[] SelectedUsersIds
        {
            get
            {
                return itemsView.GetSelectedItems().Select(x => (UserBase)((ItemView)x).Tag).Select(x=>x.Id).ToArray();
            }
            set
            {
                itemsView.GetItems().Where(x => value.Any(id => ((UserBase)((ItemView)x).Tag).Id.Equals(id))).All(x=>x.Selected=true);
            }
        }

        public event Action<UsersListView> SelectionChanged;
    }
}
