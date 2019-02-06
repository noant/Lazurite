using LazuriteUI.Icons;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LazuriteMobile.App.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MessageView : ContentView
    {
        public MessageView(string message, Icon icon)
        {
            InitializeComponent();

            labelMessage.Text = message;
            iconView.Icon = icon;
        }

        public static void Show(string message, Icon icon, Grid parentView)
        {
            var dialog = new DialogView(new MessageView(message, icon));
            dialog.Show(parentView);
        }
    }
}