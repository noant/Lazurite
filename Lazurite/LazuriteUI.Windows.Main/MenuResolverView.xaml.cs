using LazuriteUI.Icons;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Логика взаимодействия для MenuResolverView.xaml
    /// </summary>
    public partial class MenuResolverView : UserControl
    {
        public static readonly DependencyProperty ResolverProperty;

        static MenuResolverView()
        {
            ResolverProperty = DependencyProperty.Register(nameof(Resolver), typeof(ITypeResolver), typeof(MenuResolverView), new FrameworkPropertyMetadata() {
                PropertyChangedCallback = (o,e) => {
                    var type = ((ITypeResolver)e.NewValue).Type;
                    var displayName = (DisplayNameAttribute)type.GetCustomAttributes(typeof(DisplayNameAttribute), false).FirstOrDefault();
                    var icon = LazuriteIconAttribute.GetIcon(type);
                    var control = (UIElement)Activator.CreateInstance(type);

                    var resolverView = o as MenuResolverView;
                    resolverView.captionView.Content = displayName?.DisplayName;
                    resolverView.captionView.Icon = icon;
                    resolverView.contentControl.Content = control;

                    if (control is IInitializable)
                        ((IInitializable)control).Initialize();
                }
            });
        }

        public ITypeResolver Resolver
        {
            get
            {
                return (ITypeResolver)GetValue(ResolverProperty);
            }
            set
            {
                SetValue(ResolverProperty, value);
            }
        }

        public MenuResolverView()
        {
            InitializeComponent();
        }
    }
}
