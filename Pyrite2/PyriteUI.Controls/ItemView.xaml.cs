using PyriteUI.Icons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PyriteUI.Controls
{
    /// <summary>
    /// Логика взаимодействия для ItemView.xaml
    /// </summary>
    public partial class ItemView : UserControl
    {
        public static readonly DependencyProperty IconVisibilityProperty;
        public static readonly DependencyProperty IconProperty;
        public static new readonly DependencyProperty ContentProperty;
        public static readonly DependencyProperty SelectedProperty;
        public static readonly DependencyProperty SelectableProperty;

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
            SelectedProperty = DependencyProperty.Register(nameof(Selected), typeof(bool), typeof(ItemView), new FrameworkPropertyMetadata() {
                PropertyChangedCallback = (o, e) =>
                {
                    var itemView = ((ItemView)o);
                    if (itemView.Selectable)
                    {
                        var value = (bool)e.NewValue;
                        itemView.button.Background = value ? Brushes.SteelBlue : itemView._prevBackground;
                        itemView.SelectionChanged?.Invoke(o, new ItemViewSelectionChangedEventArgs()
                        {
                            ItemView = itemView,
                            Selected = value
                        });
                    }
                    else itemView.Selected = false;
                }
            });
        }

        public ItemView()
        {
            InitializeComponent();
            _prevBackground = button.Background;
            Click += (o, e) => this.Selected = !this.Selected;
        }
        
        private Brush _prevBackground;

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

        public event RoutedEventHandler Click
        {
            add
            {
                button.Click += value;
            }
            remove
            {
                button.Click -= value;
            }
        }

        public event RoutedEventHandler SelectionChanged;
    }
}
