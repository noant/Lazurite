using Lazurite.Shared;
using LazuriteUI.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendingMessagesPluginUI
{
    public class UsersView: ListItemsView
    {
        public UsersView()
        {
            this.SelectionChanged += (o, e) =>
                SelectedUsersChanged?.Invoke(this, new EventsArgs<IMessageTarget[]>(SelectedUsers));
        }

        public IMessageTarget[] Users
        {
            get => GetItems().Cast<UserItemView>().Select(x => x.User).ToArray();
            set
            {
                foreach (var user in value)
                {
                    var view = new UserItemView(user);
                    view.Click += (o,e) => 
                        SelectedUsersChanged?.Invoke(this, new EventsArgs<IMessageTarget[]>(SelectedUsers));
                    this.Children.Add(view);
                }
            }
        }

        public IMessageTarget[] SelectedUsers
        {
            get => GetSelectedItems().Cast<UserItemView>().Select(x => x.User).ToArray();
            set
            {
                foreach (UserItemView item in GetItems())
                    item.Selected = value.Any(z => z.Id == item.User.Id);
            }
        }

        public event EventsHandler<IMessageTarget[]> SelectedUsersChanged;
    }
}