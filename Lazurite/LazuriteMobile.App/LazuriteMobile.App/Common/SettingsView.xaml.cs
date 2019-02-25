using Lazurite.Shared;
using LazuriteUI.Icons;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LazuriteMobile.App.Common
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsView : Grid
    {
        public static readonly BindableProperty CaptionProperty;
        public static readonly BindableProperty DescriptionProperty;
        public static readonly BindableProperty IconProperty;
        public static readonly BindableProperty SettingsProperty;

        static SettingsView()
        {
            CaptionProperty = BindableProperty.Create(nameof(Caption), typeof(string), typeof(SettingsView), "Настройка", BindingMode.OneWay, null,
                (sender, oldVal, newVal) =>
                {
                    ((SettingsView)sender).itemView.Text = (string)newVal;
                });
            DescriptionProperty = BindableProperty.Create(nameof(Description), typeof(string), typeof(SettingsView), "Описание", BindingMode.OneWay, null,
                (sender, oldVal, newVal) =>
                {
                    ((SettingsView)sender).descriptionView.Text = (string)newVal;
                });
            IconProperty = BindableProperty.Create(nameof(Icon), typeof(Icon), typeof(SettingsView), Icon.Settings, BindingMode.OneWay, null,
                (sender, oldVal, newVal) =>
                {
                    ((SettingsView)sender).itemView.Icon = (Icon)newVal;
                });
            SettingsProperty = BindableProperty.Create(nameof(Settings), typeof(SettingsItem[]), typeof(SettingsView));
        }

        public SettingsView()
        {
            InitializeComponent();
            itemView.Clicked += ItemView_Clicked;
        }

        public string Caption
        {
            get => (string)GetValue(CaptionProperty);
            set => SetValue(CaptionProperty, value);
        }

        public string Description
        {
            get => (string)GetValue(DescriptionProperty);
            set => SetValue(DescriptionProperty, value);
        }

        public Icon Icon
        {
            get => (Icon)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public SettingsItem[] Settings
        {
            get => (SettingsItem[])GetValue(SettingsProperty);
            set => SetValue(SettingsProperty, value);
        }

        public event EventsHandler<SettingsView> CustomAction;

        private void ItemView_Clicked(object sender, Lazurite.Shared.EventsArgs<object> args)
        {
            if (CustomAction != null)
            {
                CustomAction(this, new EventsArgs<SettingsView>(this));
            }
            else
            {
                SettingsSelectView.Show(Settings, Caption, Helper.GetLastParent(this));
            }
        }
    }
}