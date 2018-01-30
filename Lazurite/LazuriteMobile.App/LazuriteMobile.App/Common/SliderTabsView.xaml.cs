using LazuriteMobile.App.Controls;
using LazuriteUI.Icons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LazuriteMobile.App.Common
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SliderTabsView : Grid
    {
        private List<TabInfo> _tabs = new List<TabInfo>();

        public SliderTabsView()
        {
            InitializeComponent();
        }

        public void AddTabInfo(TabInfo info)
        {
            var itemView = new ItemView();
            itemView.Icon = info.Icon;
            itemView.HeightRequest = itemView.WidthRequest = 45;
            itemView.BackgroundColor = Color.Transparent;
            itemView.Click += (o, e) => info.Menu.Show();
            stackPanel.Children.Add(itemView);
            _tabs.Add(info);
        }

        public void HideAll()
        {
            foreach (var tab in _tabs)
                tab.Menu.Hide();
        }

        public bool AnyOpened() => _tabs.Any(x => x.Menu.MenuVisible);

        public class TabInfo
        {
            public TabInfo(SliderMenu menu, Icon icon)
            {
                Menu = menu;
                Icon = icon;
            }

            public Icon Icon { get; }
            public SliderMenu Menu { get; }
        }
    }
}