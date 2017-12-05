using LazuriteUI.Icons;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace LazuriteUI.Windows.Controls
{
    /// <summary>
    /// Логика взаимодействия для ItemView.xaml
    /// </summary>
    public partial class ItemView : UserControl, ISelectable
    {
        public static readonly DependencyProperty IconVisibilityProperty;
        public static readonly DependencyProperty IconProperty;
        public static readonly DependencyProperty SelectedProperty;
        public static new readonly DependencyProperty ContentProperty;
        public static new readonly DependencyProperty BackgroundProperty;
        public static readonly DependencyProperty SelectableProperty;
        public static readonly DependencyProperty IconVerticalAligmentProperty;
        public static readonly DependencyProperty IconHorizontalAligmentProperty;
        public static readonly DependencyProperty CommandProperty;

        static ItemView()
        {
            IconVisibilityProperty = DependencyProperty.Register(nameof(IconVisibility), typeof(Visibility), typeof(ItemView), new FrameworkPropertyMetadata() {
                PropertyChangedCallback = (o,e) =>
                {
                    ((ItemView)o).iconView.Visibility = (Visibility)e.NewValue;
                }
            });
            IconProperty = DependencyProperty.Register(nameof(Icon), typeof(Icon), typeof(ItemView), new FrameworkPropertyMetadata()
            {
                PropertyChangedCallback = (o, e) =>
                {
                    ((ItemView)o).iconView.Icon = (Icon)e.NewValue;
                }
            });
            ContentProperty = DependencyProperty.Register(nameof(Content), typeof(object), typeof(ItemView), new FrameworkPropertyMetadata()
            {
                PropertyChangedCallback = (o, e) =>
                {
                    ((ItemView)o).label.Content = e.NewValue;
                }
            });
            SelectableProperty = DependencyProperty.Register(nameof(Selectable), typeof(bool), typeof(ItemView), new FrameworkPropertyMetadata()
            {
                PropertyChangedCallback = (o, e) =>
                {
                    var itemView = ((ItemView)o);
                    if (!(bool)e.NewValue)
                        itemView.Selected = false;
                }
            });
            SelectedProperty = DependencyProperty.Register(nameof(Selected), typeof(bool), typeof(ItemView), new FrameworkPropertyMetadata()
            {
                PropertyChangedCallback = (o, e) =>
                {
                    var itemView = ((ItemView)o);
                    if (itemView.Selectable)
                    {
                        var value = (bool)e.NewValue;
                        itemView.backView.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
                        itemView.SelectionChanged?.Invoke(o, new SelectionChangedEventArgs()
                        {
                            Item = itemView,
                            Selected = value
                        });
                    }
                    else itemView.Selected = false;
                }
            });
            IconVerticalAligmentProperty = DependencyProperty.Register(nameof(IconVerticalAligment), typeof(VerticalAlignment), typeof(ItemView), new FrameworkPropertyMetadata()
            {
                PropertyChangedCallback = (o, e) =>
                {
                    var itemView = ((ItemView)o);
                    var value = (VerticalAlignment)e.NewValue;
                    itemView.iconView.VerticalAlignment = value;
                }
            });
            IconHorizontalAligmentProperty = DependencyProperty.Register(nameof(IconHorizontalAligment), typeof(HorizontalAlignment), typeof(ItemView), new FrameworkPropertyMetadata()
            {
                PropertyChangedCallback = (o, e) =>
                {
                    var itemView = ((ItemView)o);
                    var value = (HorizontalAlignment)e.NewValue;
                    itemView.iconView.HorizontalAlignment = value;
                }
            });
            CommandProperty = DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(ItemView), new FrameworkPropertyMetadata()
            {
                PropertyChangedCallback = (o, e) =>
                {
                    var itemView = ((ItemView)o);
                    var value = (ICommand)e.NewValue;
                    itemView.button.Command = value;
                }
            });
            BackgroundProperty = DependencyProperty.Register(nameof(Background), typeof(Brush), typeof(ItemView), new FrameworkPropertyMetadata()
            {
                PropertyChangedCallback = (o, e) =>
                {
                    var element = ((UserControl)o);
                    element.Background = (Brush)e.NewValue;
                }
            });
        }

        public ItemView()
        {
            InitializeComponent();

            button.Click += (o, e) => Click?.Invoke(this, e);
            Click += (o, e) => this.Selected = !this.Selected;
            SizeChanged += (o, e) =>
            {
                if (this.ActualWidth <= 50)
                    this.label.Visibility = Visibility.Collapsed;
                else
                    this.label.Visibility = Visibility.Visible;
            };
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            Keyboard.Focus(button);
        }

        public Visibility IconVisibility
        {
            get
            {
                return (Visibility)GetValue(IconVisibilityProperty);
            }
            set
            {
                SetValue(IconVisibilityProperty, value);
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

        public new Brush Background
        {
            get
            {
                return (Brush)GetValue(BackgroundProperty);
            }
            set
            {
                SetValue(BackgroundProperty, value);
            }
        }

        public new object Content
        {
            get
            {
                return (string)GetValue(ContentProperty);
            }
            set
            {
                SetValue(ContentProperty, value);
            }
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

        public VerticalAlignment IconVerticalAligment
        {
            get
            {
                return (VerticalAlignment)GetValue(IconVerticalAligmentProperty);
            }
            set
            {
                SetValue(IconVerticalAligmentProperty, value);
            }
        }

        public HorizontalAlignment IconHorizontalAligment
        {
            get
            {
                return (HorizontalAlignment)GetValue(IconHorizontalAligmentProperty);
            }
            set
            {
                SetValue(IconHorizontalAligmentProperty, value);
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

        public event RoutedEventHandler Click;
        
        public event RoutedEventHandler SelectionChanged;
    }
}
