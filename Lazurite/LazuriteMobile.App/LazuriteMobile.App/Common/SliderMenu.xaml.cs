using LazuriteUI.Icons;

using Xamarin.Forms;

namespace LazuriteMobile.App.Common
{
    public partial class SliderMenu : Grid
    {
        public static BindableProperty ContentProperty;

        static SliderMenu()
        {
            ContentProperty = BindableProperty.Create(nameof(Content), typeof(View), typeof(SliderMenu), null, BindingMode.Default, null,
                (o, oldVal, newVal) => {
                    var view = (View)newVal;
                    var sender = ((SliderMenu)o);
                    sender.contentView.Content = view;
                });
        }

        private bool _needShow;
        private bool _initialized;

        public SliderMenu()
        {
            InitializeComponent();
            this.TranslationY = this.Height;

            this.SizeChanged += (o, e) =>
            {
                this.Hide();
                _initialized = true;
                if (_needShow)
                {
                    Show();
                    _needShow = false;
                }
            };
        }
                        
        public bool MenuVisible
        {
            get; private set;
        } = true;

        public void Hide()
        {
            this.BackgroundColor = Color.Transparent;
            this.TranslateTo(0, this.Height, 100, Easing.Linear);
            MenuVisible = false;
        }

        public void Show()
        {
            if (_initialized)
            {
                this.BackgroundColor = Color.Black;
                this.TranslateTo(0, 0, 100, Easing.Linear);
                MenuVisible = true;
                if (this.contentView.Content is IUpdatable)
                    ((IUpdatable)this.contentView.Content).UpdateView();
            }
            else _needShow = true;
        }
        
        public View Content
        {
            get
            {
                return (View)GetValue(ContentProperty);
            }
            set
            {
                SetValue(ContentProperty, value);
            }
        }
    }
}
