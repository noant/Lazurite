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
                            var selectedGroups = SelectedGroupsIds.Select(x=>_repository.Groups.First(z=>z.Name.Equals(x)));
                            foreach (var group in selectedGroups)
                                Remove(group);
                        }
                    });
            };

            this.itemsView.SelectionChanged += (o, e) =>
            {
                this.SelectionChanged?.Invoke(this);
                btRemove.IsEnabled = SelectedGroupsIds.Any();
            };
        }

        public void Add(UserGroupBase group)
        {
            AddInternal(group).Selected = true;
        }

        public void Refresh()
        {
            itemsView.Children.Clear();
            var selectedGroups = SelectedGroupsIds;

            foreach (var group in _repository.Groups)
                AddInternal(group);

            SelectedGroupsIds = selectedGroups;
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

        public string[] SelectedGroupsIds
        {
            get
            {
                return itemsView.GetSelectedItems().Select(x => (UserGroupBase)((ItemView)x).Tag).Select(x=>x.Name).ToArray();
            }
            set
            {
                itemsView.GetItems().Where(x => value.Any(name => ((UserGroupBase)((ItemView)x).Tag).Name.Equals(name))).All(x => x.Selected=true);
            }
        }

        public event Action<GroupsListView> SelectionChanged;
    }
}
