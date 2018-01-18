using Lazurite.Shared;
using LazuriteUI.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserGeolocationPluginUI
{
    public class UsersView: ListItemsView
    {
        public UsersView()
        {
            this.SelectionMode = ListViewItemsSelectionMode.Single;
            this.SelectionChanged += (o, e) => {
                SelectedUserChanged?.Invoke(this, new EventsArgs<IGeolocationTarget>(this.SelectedUser));
            };
        }

        public IGeolocationTarget[] Users
        {
            get
            {
                return this.GetItems().Cast<UserItemView>().Select(x => x.User).ToArray();
            }
            set
            {
                this.Children.Clear();
                if (value != null)
                    foreach (var user in value)
                        this.Children.Add(new UserItemView(user));
            }
        }

        public IGeolocationTarget SelectedUser
        {
            get
            {
                return (this.GetSelectedItems().FirstOrDefault() as UserItemView)?.User;
            }
            set
            {
                this.GetItems()
                    .Cast<UserItemView>()
                    .Where(x => x.User.Equals(value))
                    .All(x => x.Selected = true);
            }
        }

        public event EventsHandler<IGeolocationTarget> SelectedUserChanged;
    }
}
