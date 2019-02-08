using LazuriteMobile.App.Controls;
using LazuriteUI.Icons;
using System.Collections.Generic;
using System.Linq;

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
            var itemView = new ItemView() {
                BackgroundColor = Color.Transparent,
                Icon = info.Icon,
                HeightRequest = 45,
                WidthRequest = 45,
                Opacity = Visual.Current.BottomPanelIconOpacity,
                IconForeground = Visual.Current.BottomPanelIconColor
            };
            itemView.Clicked += (o, e) => info.Menu.Show();
            stackPanel.Children.Add(itemView);
            _tabs.Add(info);
        }

        public async void HideAll()
        {
            foreach (var tab in _tabs)
                await tab.Menu.Hide();
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