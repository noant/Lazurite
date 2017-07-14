using LazuriteUI.Icons;
using LazuriteUI.Windows.Controls;
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

namespace LazuriteUI.Windows.Main
{
    /// <summary>
    /// Логика взаимодействия для MenuItemView.xaml
    /// </summary>
    public partial class MenuItemView : UserControl, ISelectable, IViewTypeResolverItem
    {
        public static readonly DependencyProperty IconProperty;
        public static readonly DependencyProperty TextProperty;
        public static readonly DependencyProperty SelectedProperty;
        public static readonly DependencyProperty SelectableProperty;
        public static readonly DependencyProperty TypeProperty;

        static MenuItemView()
        {
            SelectedProperty = DependencyProperty.Register(nameof(Selected), typeof(bool), typeof(MenuItemView), new FrameworkPropertyMetadata() {
                PropertyChangedCallback = (o,e) => {
                    ((MenuItemView)o).itemView.Selected = (bool)e.NewValue;
                    ((MenuItemView)o).RaiseSelectionChanged();
                }
            });
            SelectableProperty = DependencyProperty.Register(nameof(Selectable), typeof(bool), typeof(MenuItemView), new FrameworkPropertyMetadata(true)
            {
                PropertyChangedCallback = (o, e) => {
                    ((MenuItemView)o).itemView.Selectable = (bool)e.NewValue;
                }
            });
            IconProperty = DependencyProperty.Register(nameof(Icon), typeof(Icon), typeof(MenuItemView));
            TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(MenuItemView));
            TypeProperty = DependencyProperty.Register(nameof(Type), typeof(Type), typeof(MenuItemView));
        }

        public MenuItemView()
        {
            InitializeComponent();
            itemView.SelectionChanged += (o, e) => {
                this.Selected = itemView.Selected;
            };
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

        public object Text
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

        public Type Type
        {
            get
            {
                return (Type)GetValue(TypeProperty);
            }
            set
            {
                SetValue(TypeProperty, value);
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

        private void RaiseSelectionChanged()
        {
            SelectionChanged?.Invoke(this, new RoutedEventArgs());
        }

        public event RoutedEventHandler Click
        {
            add
            {
                itemView.Click += value;
            }
            remove
            {
                itemView.Click -= value;
            }
        }

        public event RoutedEventHandler SelectionChanged;
    }
}
