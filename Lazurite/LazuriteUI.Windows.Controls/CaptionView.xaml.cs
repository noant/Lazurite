using LazuriteUI.Icons;
using System.Windows;
using System.Windows.Controls;

namespace LazuriteUI.Windows.Controls
{
    /// <summary>
    /// Логика взаимодействия для CaptionView.xaml
    /// </summary>
    public partial class CaptionView : UserControl
    {
        public static readonly DependencyProperty IconProperty;
        public static new readonly DependencyProperty ContentProperty;

        static CaptionView()
        {
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
            progressBar.StartProgress();
        }

        public void StopAnimateProgress()
        {
            progressBar.StopProgress();
        }

        public CaptionView()
        {
            InitializeComponent();
        }
    }
}