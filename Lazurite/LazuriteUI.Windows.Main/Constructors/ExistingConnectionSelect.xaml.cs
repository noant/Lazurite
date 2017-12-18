using Lazurite.MainDomain;
using Lazurite.Shared;
using LazuriteUI.Icons;
using LazuriteUI.Windows.Controls;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace LazuriteUI.Windows.Main.Constructors
{
    /// <summary>
    /// Логика взаимодействия для RemoteScenarioSelect.xaml
    /// </summary>
    public partial class ExistingConnectionSelect : UserControl
    {
        public ExistingConnectionSelect(ConnectionCredentials[] credentials)
        {
            InitializeComponent();

            foreach (var cred in credentials)
            {
                var itemView = new ItemView();
                itemView.Content = cred.GetAddress() + "@" + cred.Login;
                itemView.Icon = Icon.ChevronRight;
                itemView.Tag = cred;
                itemView.Margin = new Thickness(2);
                listItems.Children.Add(itemView);
            }

            listItems.SelectionChanged += (o, e) =>
            {
                if (listItems.GetSelectedItems().Any())
                    SelectedCredentialsChanged?.Invoke(this, new EventsArgs<ConnectionCredentials>((ConnectionCredentials)((ItemView)listItems.SelectedItem).Tag));
            };
        }

        public event EventsHandler<ConnectionCredentials> SelectedCredentialsChanged;
    }
}
