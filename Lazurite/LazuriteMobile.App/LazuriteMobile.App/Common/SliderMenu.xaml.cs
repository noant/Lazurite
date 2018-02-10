using Lazurite.Shared;
using LazuriteUI.Icons;
using System;
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
            BackgroundColor = Color.Black;
            TranslationY = Height;

            SizeChanged += (o, e) =>
            {
                Hide();
                _initialized = true;
                if (_needShow)
                {
                    Show();
                    _needShow = false;
                }
            };
        }

        public bool MenuVisible { get; private set; } = true;

        public void Hide()
        {
            this.TranslateTo(0, Height, 100, Easing.Linear);
            MenuVisible = false;
        }

        public void Show()
        {
            if (BeforeMenuShown != null)
                BeforeMenuShown(
                    this, 
                    new EventsArgs<ShowMenuContinuation>(new ShowMenuContinuation(ShowInternal)));
            else
                ShowInternal();
        }

        private void ShowInternal()
        {
            if (_initialized)
            {
                if (contentView.Content is IUpdatable)
                    ((IUpdatable)contentView.Content).UpdateView(()=> {
                        this.TranslateTo(0, 0, 100, Easing.Linear);
                        MenuVisible = true;
                    });
                else
                {
                    this.TranslateTo(0, 0, 100, Easing.Linear);
                    MenuVisible = true;
                }
            }
            else _needShow = true;
        }

        public event EventsHandler<ShowMenuContinuation> BeforeMenuShown;

        public View Content
        {
            get => (View)GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }
    }

    public class ShowMenuContinuation
    {
        public ShowMenuContinuation(Action @continue)
        {
            _continue = @continue;
        }

        private Action _continue;

        public void ContinueShow() => _continue?.Invoke();
    }
}
