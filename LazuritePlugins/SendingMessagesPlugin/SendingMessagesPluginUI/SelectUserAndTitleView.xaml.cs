using Lazurite.Shared;
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

namespace SendingMessagesPluginUI
{
    /// <summary>
    /// Логика взаимодействия для SelectUserAndTitleView.xaml
    /// </summary>
    public partial class SelectUserAndTitleView : UserControl
    {
        public static DependencyProperty SelectionModeProperty;
        public static DependencyProperty UsersEnabledProperty;

        static SelectUserAndTitleView()
        {
            SelectionModeProperty =
                DependencyProperty.Register(nameof(SelectionMode), typeof(ListViewItemsSelectionMode), typeof(SelectUserAndTitleView), new FrameworkPropertyMetadata(ListViewItemsSelectionMode.None)
                {
                    PropertyChangedCallback = (o,e) => {
                        ((SelectUserAndTitleView)o).usersView.SelectionMode = (ListViewItemsSelectionMode)e.NewValue;
                    }
                });
            UsersEnabledProperty =
                DependencyProperty.Register(nameof(UsersEnabled), typeof(bool), typeof(SelectUserAndTitleView), new FrameworkPropertyMetadata(true)
                {
                    PropertyChangedCallback = (o, e) => {
                        ((SelectUserAndTitleView)o).usersView.IsEnabled = (bool)e.NewValue;
                    }
                });
        }

        public SelectUserAndTitleView()
        {
            InitializeComponent();
        }

        public ListViewItemsSelectionMode SelectionMode
        {
            get => (ListViewItemsSelectionMode)GetValue(SelectionModeProperty);
            set => SetValue(SelectionModeProperty, value);
        }

        public bool UsersEnabled
        {
            get => (bool)GetValue(UsersEnabledProperty);
            set => SetValue(UsersEnabledProperty, value);
        }

        public void SetUsers(IMessageTarget[] users)
        {
            this.usersView.Users = users;
        }

        public IMessageTarget[] SelectedUsers
        {
            get => usersView.SelectedUsers;
            set => usersView.SelectedUsers = value;
        }

        public event EventsHandler<IMessageTarget[]> SelectedUsersChanged
        {
            add => usersView.SelectedUsersChanged += value;
            remove => usersView.SelectedUsersChanged -= value;
        }

        public string Title
        {
            get => tbTitle.Text;
            set => tbTitle.Text = value;
        }
    }
}
