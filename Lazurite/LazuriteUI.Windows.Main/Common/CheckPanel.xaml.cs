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

namespace LazuriteUI.Windows.Main.Common
{
    /// <summary>
    /// Логика взаимодействия для CheckPanel.xaml
    /// </summary>
    public partial class CheckPanel : UserControl
    {
        public static DependencyProperty CheckedProperty;

        static CheckPanel()
        {
            CheckedProperty = DependencyProperty.Register(nameof(Checked), typeof(bool), typeof(CheckPanel),
                new FrameworkPropertyMetadata() {
                    DefaultValue = false,
                    PropertyChangedCallback = (o, e) => {
                        var checkPanel = o as CheckPanel;
                        checkPanel.borderChecked.Visibility = (bool)e.NewValue ? Visibility.Visible : Visibility.Collapsed;
                    }
                });
        }

        public CheckPanel()
        {
            InitializeComponent();
        }

        public bool Checked {
            get
            {
                return (bool)GetValue(CheckedProperty);
            }
            set
            {
                SetValue(CheckedProperty, value);
            }
        }
    }
}
