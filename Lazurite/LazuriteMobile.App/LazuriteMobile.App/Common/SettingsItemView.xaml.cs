using Lazurite.Shared;
using LazuriteUI.Icons;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LazuriteMobile.App.Common
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsItemView : Grid
    {
        public SettingsItemView(SettingsItem item)
        {
            InitializeComponent();
            captionView.Text = item.Caption;
            descriptionView.Text = item.Description;
            if (item.IsSelected())
            {
                btApply.Text = "Применено";
                btApply.Icon = Icon.Check;
                btApply.Selected = true;
                btApply.InputTransparent = true;
            }
            else
            {
                btApply.Click += (o, e) =>
                {
                    Clicked?.Invoke(this, new EventsArgs<SettingsItem>(item));
                    item.RaiseAction();
                };
            }
        }

        public event EventsHandler<SettingsItem> Clicked;
    }
}