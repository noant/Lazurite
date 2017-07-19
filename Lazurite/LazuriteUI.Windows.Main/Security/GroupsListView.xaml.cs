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
    /// Логика взаимодействия для UsersListView.xaml
    /// </summary>
    public partial class GroupsListView : UserControl
    {
        private UsersRepositoryBase _repository = Singleton.Resolve<UsersRepositoryBase>();

        public GroupsListView()
        {
            InitializeComponent();

            foreach (var group in _repository.Groups)
                AddInternal(group);

            this.itemsView.SelectionChanged += (o, e) => this.SelectionChanged?.Invoke(this);
        }

        public void Add(UserGroupBase group)
        {
            AddInternal(group).Selected = true;
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
                itemsView.GetItems().Where(x => value.Contains((UserGroupBase)((ItemView)x).Tag)).All(x=>x.Selected=true);
            }
        }

        public event Action<GroupsListView> SelectionChanged;
    }
}
