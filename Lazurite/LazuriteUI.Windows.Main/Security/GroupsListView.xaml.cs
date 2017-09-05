using Lazurite.IOC;
using Lazurite.MainDomain;
using Lazurite.Security;
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
    /// Логика взаимодействия для UsersListView.xaml
    /// </summary>
    public partial class GroupsListView : Grid
    {
        private UsersRepositoryBase _repository = Singleton.Resolve<UsersRepositoryBase>();

        public GroupsListView()
        {
            InitializeComponent();

            Refresh();

            btAdd.Click += (o, e) => {
                var group = new UserGroup();
                AddGroupView.Show(
                        () => {
                            _repository.Add(group);
                            AddInternal(group);
                        },
                        (args) => args.Success = !_repository.Groups.Any(x => x.Name.Equals(args.Name)),
                        group
                    );
            };

            btRemove.Click += (o, e) => 
            {
                MessageView.ShowYesNo("Вы уверены что хотите удалить выбранные группы?", "Удаление групп", Icons.Icon.GroupDelete,
                    (result) => {
                        if (result)
                        {
                            var selectedGroups = SelectedGroups;
                            foreach (var group in selectedGroups)
                                Remove(group);
                        }
                    });
            };

            this.itemsView.SelectionChanged += (o, e) =>
            {
                this.SelectionChanged?.Invoke(this);
                btRemove.IsEnabled = SelectedGroups.Any();
            };
        }

        public void Add(UserGroupBase group)
        {
            AddInternal(group).Selected = true;
        }

        public void Refresh()
        {
            itemsView.Children.Clear();
            var selectedGroups = SelectedGroups;

            foreach (var group in _repository.Groups)
                AddInternal(group);

            SelectedGroups = selectedGroups;
            btRemove.IsEnabled = selectedGroups.Any();
        }

        public void Refresh(UserGroupBase group)
        {
            var itemView = itemsView.Children.Cast<ItemView>().Single(x => ((UserGroupBase)x.Tag).Name.Equals(group.Name));
            RefreshInternal(itemView, group);
        }

        public void Remove(UserGroupBase group)
        {
            var itemView = itemsView.Children.Cast<ItemView>().Single(x => ((UserGroupBase)x.Tag).Name.Equals(group.Name));
            itemsView.Children.Remove(itemView);
            _repository.Remove(group);
        }

        private ItemView AddInternal(UserGroupBase group)
        {
            var itemView = new ItemView();
            RefreshInternal(itemView, group);
            itemsView.Children.Add(itemView);
            return itemView;
        }

        private void RefreshInternal(ItemView itemView, UserGroupBase group)
        {
            itemView.Icon = Icons.Icon.ChevronRight;
            itemView.Content = group.Name;
            itemView.Margin = new Thickness(0, 1, 0, 0);
            itemView.Tag = group;
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

        public UserGroupBase[] SelectedGroups
        {
            get
            {
                return itemsView.GetSelectedItems().Select(x => (UserGroupBase)((ItemView)x).Tag).ToArray();
            }
            set
            {
                itemsView.GetItems().Where(x => value.Any(group=>((UserGroupBase)((ItemView)x).Tag).Name.Equals(group.Name))).All(x=>x.Selected=true);
            }
        }

        public event Action<GroupsListView> SelectionChanged;
    }
}
