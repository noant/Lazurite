using Lazurite.Shared;
using LazuriteMobile.App.Controls;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace LazuriteMobile.App.Common
{
    public partial class SliderMenu : Grid, IDialogViewHost
    {
        public static BindableProperty ContentProperty;

        static SliderMenu()
        {
            ContentProperty = BindableProperty.Create(nameof(Content), typeof(View), typeof(SliderMenu), null, BindingMode.Default, null,
                (o, oldVal, newVal) =>
                {
                    var view = (View)newVal;
                    var sender = (SliderMenu)o;
                    sender.contentView.Content = view;
                });
        }

        private bool _needShow;
        private bool _initialized;

        public SliderMenu()
        {
            InitializeComponent();
            TranslationY = Height;

            if (Device.Idiom == TargetIdiom.Tablet ||
                Device.Idiom == TargetIdiom.TV)
            {
                backGrid.IsVisible = true;
                SetColumn(contentView, 1);
                SetColumnSpan(contentView, 1);
            }

            SizeChanged += async (o, e) =>
            {
                await Hide();
                _initialized = true;
                if (_needShow)
                {
                    Show();
                    _needShow = false;
                }
            };
        }

        public bool MenuVisible { get; private set; } = true;

        public async Task Hide()
        {
            MenuVisible = false;
            await this.TranslateTo(0, Height, 100, Easing.Linear);
        }

        public void Show()
        {
            if (BeforeMenuShown != null)
            {
                BeforeMenuShown(
                    this,
                    new EventsArgs<ShowMenuContinuation>(new ShowMenuContinuation(ShowInternal)));
            }
            else
            {
                ShowInternal();
            }
        }

        private void ShowInternal()
        {
            if (_initialized)
            {
                if (contentView.Content is IUpdatable)
                {
                    ((IUpdatable)contentView.Content).UpdateView(() =>
                    {
                        this.TranslateTo(0, 0, 100, Easing.CubicIn);
                        MenuVisible = true;
                    });
                }
                else
                {
                    this.TranslateTo(0, 0, 100, Easing.CubicIn);
                    MenuVisible = true;
                }
            }
            else
            {
                _needShow = true;
            }
        }

        public event EventsHandler<ShowMenuContinuation> BeforeMenuShown;

        public View Content
        {
            get => (View)GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }

        public int Column
        {
            get
            {
                if (Device.Idiom == TargetIdiom.Tablet ||
                    Device.Idiom == TargetIdiom.TV)
                {
                    return 1;
                }
                return 0;
            }
        }

        public int Row => 0;

        public int ColumnSpan
        {
            get
            {
                if (Device.Idiom == TargetIdiom.Tablet ||
                    Device.Idiom == TargetIdiom.TV)
                {
                    return 1;
                }
                return 3;
            }
        }

        public int RowSpan => 1;
    }

    public class ShowMenuContinuation
    {
        public ShowMenuContinuation(Action @continue)
        {
            _continue = @continue;
        }

        private readonly Action _continue;

        public void ContinueShow() => _continue?.Invoke();
    }
}