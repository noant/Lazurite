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
            ResolverProperty = DependencyProperty.Register(nameof(Resolver), typeof(IViewTypeResolverItem), typeof(MenuResolverView), new FrameworkPropertyMetadata() {
                PropertyChangedCallback = (o,e) => {
                    var resolverView = o as MenuResolverView;
                    var @continue = new Action(() => {
                        var type = ((IViewTypeResolverItem)e.NewValue).Type;
                        var displayName = (DisplayNameAttribute)type.GetCustomAttributes(typeof(DisplayNameAttribute), false).FirstOrDefault();
                        var icon = LazuriteIconAttribute.GetIcon(type);
                        var control = (UIElement)Activator.CreateInstance(type);

                        resolverView.captionView.Content = displayName?.DisplayName;
                        resolverView.captionView.Icon = icon;
                        resolverView.contentControl.Content = control;

                        if (control is IInitializable)
                            ((IInitializable)control).Initialize();
                    });
                    if (resolverView.contentControl.Content is IAllowSave)
                        ((IAllowSave)resolverView.contentControl.Content).Save(@continue);
                    else @continue();
                }
            });
        }

        public IViewTypeResolverItem Resolver
        {
            get
            {
                return (IViewTypeResolverItem)GetValue(ResolverProperty);
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
