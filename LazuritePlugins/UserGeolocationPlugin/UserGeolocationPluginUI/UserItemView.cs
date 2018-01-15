using Lazurite.Shared;
using LazuriteUI.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserGeolocationPluginUI
{
    public class UserItemView : ItemView
    {
        public IGeolocationTarget User { get; private set; }

        public UserItemView(IGeolocationTarget user)
        {
            User = user;
            this.Icon = LazuriteUI.Icons.Icon.ChevronRight;
            this.Content = user.Name;
            this.Margin = new System.Windows.Thickness(0, 1, 0, 0);
        }
    }
}
