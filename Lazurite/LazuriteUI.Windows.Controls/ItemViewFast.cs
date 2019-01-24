using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace LazuriteUI.Windows.Controls
{
    /// <summary>
    /// Логика взаимодействия для ItemView.xaml
    /// </summary>
    public partial class ItemViewFast : UserControl, ISelectable
    {
        public const int CharWidth = 7;

        private static ControlTemplate EmptyButtonTemplate = new ControlTemplate(typeof(Button))
        {
            VisualTree = new FrameworkElementFactory(typeof(ContentPresenter))
        };

        public static readonly DependencyProperty SelectedProperty;
        public static readonly DependencyProperty TextProperty;
        public static readonly DependencyProperty SelectableProperty;
        public static readonly DependencyProperty CommandProperty;

        static ItemViewFast()
        {
            TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(ItemViewFast), new FrameworkPropertyMetadata()
            {
                PropertyChangedCallback = (o, e) =>
                {
                    (o as ItemViewFast).ApplyText();
                }
            });
            SelectableProperty = DependencyProperty.Register(nameof(Selectable), typeof(bool), typeof(ItemViewFast), new FrameworkPropertyMetadata()
            {
                PropertyChangedCallback = (o, e) =>
                {
                    var itemView = ((ItemViewFast)o);
                    if (!(bool)e.NewValue)
                        itemView.Selected = false;
                }
            });
            SelectedProperty = DependencyProperty.Register(nameof(Selected), typeof(bool), typeof(ItemViewFast), new FrameworkPropertyMetadata()
            {
                PropertyChangedCallback = (o, e) =>
                {
                    var itemView = ((ItemViewFast)o);
                    if (itemView.Selectable)
                    {
                        var value = (bool)e.NewValue;
                        var grid = (itemView.Content as Button).Content as Grid;
                        grid.Background = value ? Visual.ItemSelection : Visual.ItemBackground;
                        itemView.SelectionChanged?.Invoke(o, new SelectionChangedEventArgs()
                        {
                            Item = itemView,
                            Selected = value
                        });
                    }
                    else itemView.Selected = false;
                }
            });
            CommandProperty = DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(ItemViewFast), new FrameworkPropertyMetadata()
            {
                PropertyChangedCallback = (o, e) =>
                {
                    var itemView = ((ItemViewFast)o);
                    var value = (ICommand)e.NewValue;
                    itemView.button.Command = value;
                }
            });
        }

        private Button button;
        private TextBlock tblock;

        public ItemViewFast()
        {
            InitializeComponent();
            
            button.Click += (o, e) => Click?.Invoke(this, e);
            Click += (o, e) => Selected = !Selected;
            SizeChanged += (o, e) =>
            {
                if (VerticalAlignment == VerticalAlignment.Stretch)
                    Height = double.NaN;
                ApplyText();
            };
        }
        
        private void ApplyText()
        {
            if (Text != null && Text.Length != 0 && ActualWidth > 0 && ActualWidth > 0)
            {
                var textWidth = Text.Length * CharWidth;
                var txt = Text
                    .Replace("\r", string.Empty)
                    .Replace("\n", string.Empty)
                    .ToString().Replace("_", "__"); // Сrutch for "mnemonic key using"
                if (textWidth > ActualWidth)
                    tblock.Text = Text.Substring(0, (int)(ActualWidth / CharWidth) - 2) + "...";
                else tblock.Text = Text;
            }
        }

        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            if (oldParent is FrameworkElement p)
                p.SizeChanged -= P_SizeChanged;
            if (Parent is FrameworkElement p1)
            {
                p1.SizeChanged += P_SizeChanged;
                if (!double.IsNaN(p1.ActualWidth) && p1.ActualWidth > 0)
                {
                    MaxWidth = p1.ActualWidth;
                    ApplyText();
                }
            }
        }

        private void P_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var p = sender as FrameworkElement;
            if (!double.IsNaN(p.ActualWidth) && p.ActualWidth > 0)
            {
                MaxWidth = p.ActualWidth;
                ApplyText();
            }
        }

        private void InitializeComponent()
        {
            // Not xaml because i need inherit from this class

            Height = 30;
            Width = double.NaN;
            IsHitTestVisible = true;
            Focusable = true;
            VerticalAlignment = VerticalAlignment.Top;
            FocusManager.SetFocusedElement(this, button);

            var grid = new Grid() {
                Background = Visual.ItemBackground
            };

            button = new Button()
            {
                Name = "button",
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                VerticalContentAlignment = VerticalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            
            button.MouseEnter += (o, e) => grid.Background = Brushes.DarkSlateBlue;
            button.MouseLeave += (o, e) => grid.Background = !Selected ? Visual.ItemBackground : Visual.ItemSelection;

            button.Template = EmptyButtonTemplate;

            tblock = new TextBlock()
            {
                IsHitTestVisible = false,
                Background = Brushes.Transparent,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Name = "label",
                Foreground = Visual.Foreground,
                FontSize = Visual.FontSize
            };
            grid.Children.Add(tblock);

            var ellipse = new Ellipse();
            ellipse.Fill = Brushes.LightSlateGray;
            ellipse.Height = ellipse.Width = 6;
            ellipse.Margin = new Thickness(12);
            ellipse.HorizontalAlignment = HorizontalAlignment.Left;
            ellipse.VerticalAlignment = VerticalAlignment.Center;
            grid.Children.Add(ellipse);

            button.Content = grid;

            Content = button;
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            Keyboard.Focus(button);
        }
        
        public bool Selected
        {
            get
            {
                return (bool)GetValue(SelectedProperty);
            }
            set
            {
                SetValue(SelectedProperty, value);
            }
        }

        public bool Selectable
        {
            get
            {
                return (bool)GetValue(SelectableProperty);
            }
            set
            {
                SetValue(SelectableProperty, value);
            }
        }
        
        public ICommand Command
        {
            get
            {
                return (ICommand)GetValue(CommandProperty);
            }
            set
            {
                SetValue(CommandProperty, value);
            }
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

        public event RoutedEventHandler Click;

        public event RoutedEventHandler SelectionChanged;
    }
}
