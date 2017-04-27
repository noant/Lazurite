using LazuriteUI.Icons;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LazuriteUI.Controls
{
    /// <summary>
    /// Логика взаимодействия для CaptionView.xaml
    /// </summary>
    public partial class CaptionView : UserControl
    {
        public static readonly DependencyProperty IconVisibilityProperty;
        public static readonly DependencyProperty IconProperty;
        public static new readonly DependencyProperty ContentProperty;

        static CaptionView()
        {
            IconVisibilityProperty = DependencyProperty.Register(nameof(IconVisibility), typeof(Visibility), typeof(CaptionView), new FrameworkPropertyMetadata()
            {
                PropertyChangedCallback = (o, e) =>
                {
                    ((CaptionView)o).iconView.Visibility = (Visibility)e.NewValue;
                }
            });
            IconProperty = DependencyProperty.Register(nameof(Icon), typeof(Icon), typeof(CaptionView), new FrameworkPropertyMetadata()
            {
                PropertyChangedCallback = (o, e) =>
                {
                    ((CaptionView)o).iconView.Icon = (Icon)e.NewValue;
                }
            });
            ContentProperty = DependencyProperty.Register(nameof(Content), typeof(object), typeof(CaptionView), new FrameworkPropertyMetadata()
            {
                PropertyChangedCallback = (o, e) =>
                {
                    ((CaptionView)o).label.Content = e.NewValue;
                }
            });
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

        public void StartAnimateProgress()
        {
            progressGrid.Visibility = Visibility.Visible;
            ((Storyboard)progressGrid.Resources["progressGridAnimation"]).Begin(progressGrid);
        }

        public void StopAnimateProgress()
        {
            progressGrid.Visibility = Visibility.Collapsed;
            ((Storyboard)progressGrid.Resources["progressGridAnimation"]).Stop(progressGrid);
        }

        public CaptionView()
        {
            InitializeComponent();
        }
    }
}