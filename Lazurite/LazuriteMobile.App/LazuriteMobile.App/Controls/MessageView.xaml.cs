using LazuriteUI.Icons;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LazuriteMobile.App.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MessageView : Grid
    {
        public static readonly BindableProperty IconProperty;
        public static readonly BindableProperty TextProperty;

        static MessageView()
        {
            TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(MessageView), "Message", BindingMode.OneWay, null,
                (sender, oldVal, newVal) =>
                {
                    ((MessageView)sender).labelMessage.Text = (string)newVal;
                });
            IconProperty = BindableProperty.Create(nameof(Icon), typeof(Icon), typeof(MessageView), Icon.Notification, BindingMode.OneWay, null,
                (sender, oldVal, newVal) =>
                {
                    ((MessageView)sender).iconView.Icon = (Icon)newVal;
                });
        }

        public MessageView(string message, Icon icon)
        {
            InitializeComponent();

            Text = message;
            Icon = icon;
        }

        public string Text
        {
            get
            {
                return (string)GetValue(TextProperty);
            }
            set
            {
                SetValue(TextProperty, value);
            }
        }

        public Icon Icon
        {
            get
            {
                return (Icon)GetValue(IconProperty);
            }
            set
            {
                SetValue(IconProperty, value);
            }
        }

        public MessageView()
        {
            InitializeComponent();
        }

        public static DialogView Show(string message, Icon icon, Grid parentView)
        {
            var view = new MessageView(message, icon);
            view.bgGrid.IsVisible = false; // Crutch
            var dialog = new DialogView(view);
            dialog.Show(parentView);
            return dialog;
        }
    }
}