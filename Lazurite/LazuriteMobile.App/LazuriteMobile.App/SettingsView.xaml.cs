using Lazurite.Shared;
using LazuriteMobile.App.Common;
using LazuriteMobile.App.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LazuriteMobile.App
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsView : Grid
    {
        public SettingsView()
        {
            InitializeComponent();
            BindingContext = this;
        }

        public void SelectSkin(object sender, EventsArgs<SettingsView> sv)
        {
            SkinSelectView.Show(Helper.GetLastParent(this));
        }

        public void SelectSetting(SettingsItem settingItem)
        {
        }
    }
}