using LazuriteUI.Icons;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
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
        public static readonly DependencyProperty SelectedProperty;
        public static readonly DependencyProperty IconProperty;
        public static new readonly DependencyProperty ContentProperty;
        public static readonly DependencyProperty SelectableProperty;
        public static readonly DependencyProperty IconVerticalAligmentProperty;
        public static readonly DependencyProperty IconHorizontalAligmentProperty;
        public static readonly DependencyProperty SelectionBrushProperty;
        public static readonly DependencyProperty CommandProperty;

        static ItemView()
        {
            IconVisibilityProperty = DependencyProperty.Register(nameof(IconVisibility), typeof(Visibility), typeof(ItemView), new FrameworkPropertyMetadata()
            {
                PropertyChangedCallback = (o, e) =>
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
            IconHorizontalAligmentProperty = DependencyProperty.Register(nameof(IconHorizontalAligment), typeof(HorizontalAlignment), typeof(ItemView), new FrameworkPropertyMetadata(HorizontalAlignment.Left)
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
            BackgroundProperty.OverrideMetadata(typeof(ItemView), new FrameworkPropertyMetadata(Visual.ItemBackground)
            {
                PropertyChangedCallback = (o, e) =>
                {
                    var element = ((UserControl)o);
                    element.Background = (Brush)e.NewValue;
                }
            });
            SelectionBrushProperty = DependencyProperty.Register(nameof(SelectionBrush), typeof(Brush), typeof(ItemView), new FrameworkPropertyMetadata(Visual.ItemSelection)
            {
                PropertyChangedCallback = (o, e) =>
                {
                    var element = (ItemView)o;
                    element.backView.Background = (Brush)e.NewValue;
                }
            });
        }

        private Button button;
        private Label label;
        private IconView iconView;
        private Grid backView;

        public ItemView()
        {
            InitializeComponent();

            button.Click += (o, e) => Click?.Invoke(this, e);
            Click += (o, e) => Selected = !Selected;
            SizeChanged += (o, e) =>
            {
                if (ActualWidth <= 50)
                    label.Visibility = Visibility.Collapsed;
                else
                    label.Visibility = Visibility.Visible;
            };
        }

        private void InitializeComponent()
        {
            //not xaml because i need inherit from this class

            Resources = new ResourceDictionary() { Source = new System.Uri("/LazuriteUI.Windows.Controls;component/Styles/Styles.xaml", System.UriKind.Relative ) };
            
            Height = 30;
            Width = double.NaN;
            IsHitTestVisible = true;
            Focusable = true;
            FocusManager.SetFocusedElement(this, button);

            var grid = new Grid();

            button = new Button()
            {
                Name = "button",
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Style = (Style)FindResource("ItemButtonStyle")
            };

            label = new Label()
            {
                IsHitTestVisible = false,
                Background = Brushes.Transparent,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Name = "label",
                Style = (Style)FindResource("LabelStyle")
            };

            iconView = new IconView()
            {
                IsHitTestVisible = false,
                Name = "iconView",
                Margin = new Thickness(4),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center
            };
            iconView.SetBinding(IconView.VisibilityProperty, new Binding("IconVisibility"));

            backView = new Grid()
            {
                Visibility = Visibility.Collapsed,
                Name = "backView",
                Background = Brushes.SteelBlue
            };

            grid.Children.Add(backView);
            grid.Children.Add(label);
            grid.Children.Add(iconView);

            button.Content = grid;

            base.Content = button;
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

        public Brush SelectionBrush
        {
            get
            {
                return (Brush)GetValue(SelectionBrushProperty);
            }
            set
            {
                SetValue(SelectionBrushProperty, value);
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
