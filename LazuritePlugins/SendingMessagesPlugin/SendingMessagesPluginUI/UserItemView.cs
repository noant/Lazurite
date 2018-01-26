using Lazurite.Shared;
using LazuriteUI.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendingMessagesPluginUI
{
    public class UserItemView: ItemView
    {
        public IMessageTarget User { get; private set; }

        public UserItemView(IMessageTarget user)
        {
            User = user;
            Content = user.Name;
            Icon = LazuriteUI.Icons.Icon.User;
            Margin = new System.Windows.Thickness(0, 1, 0, 0);
        }
    }
}
