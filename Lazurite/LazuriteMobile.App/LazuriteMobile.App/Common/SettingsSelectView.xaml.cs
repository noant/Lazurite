using Lazurite.Shared;
using LazuriteMobile.App.Controls;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LazuriteMobile.App.Common
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsSelectView : ContentView
    {
        public SettingsSelectView(SettingsItem[] items, string caption)
        {
            InitializeComponent();
            captionView.Text = caption;
            foreach (var item in items)
            {
                var itemView = new SettingsItemView(item);
                itemView.Clicked += ItemView_Clicked;
                stackView.Children.Add(itemView);
            }
        }

        private void ItemView_Clicked(object sender, EventsArgs<SettingsItem> args)
        {
            ItemClicked?.Invoke(this, args);
        }

        public event EventsHandler<SettingsItem> ItemClicked;

        public static void Show(SettingsItem[] items, string caption, Grid parent)
        {
            var selectView = new SettingsSelectView(items, caption);
            var dialogView = new DialogView(selectView);
            selectView.ItemClicked += (o, e) => dialogView.Close();
            dialogView.Show(parent);
        }
    }

    public class SettingsItem
    {
        public Action<SettingsItem> Action { get; set; }
        public object Tag { get; set; }
        public Func<SettingsItem, bool> IsSelected { get; set; }
        public string Caption { get; set; }
        public string Description { get; set; }
    }
}