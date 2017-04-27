﻿using LazuriteUI.Icons;
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

namespace LazuriteUI.Controls
{
    /// <summary>
    /// Логика взаимодействия для ItemView.xaml
    /// </summary>
    public partial class ItemView : UserControl, ISelectable
    {
        public static readonly DependencyProperty IconVisibilityProperty;
        public static readonly DependencyProperty IconProperty;
        public static new readonly DependencyProperty ContentProperty;
        public static readonly DependencyProperty SelectedProperty;
        public static readonly DependencyProperty SelectableProperty;
        public static readonly DependencyProperty IconVerticalAligmentProperty;
        public static readonly DependencyProperty IconHorizontalAligmentProperty;

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
            IconVerticalAligmentProperty = DependencyProperty.Register(nameof(IconVerticalAligment), typeof(VerticalAlignment), typeof(ItemView), new FrameworkPropertyMetadata() {
                PropertyChangedCallback = (o,e) =>
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
        }

        public ItemView()
        {
            InitializeComponent();
            button.Click += (o, e) => Click?.Invoke(this, e);
            Click += (o, e) => this.Selected = !this.Selected;
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

        public event RoutedEventHandler Click;
        
        public event RoutedEventHandler SelectionChanged;
    }
}