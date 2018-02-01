using System.Windows.Controls;

namespace LazuriteUI.Windows.Main
{
    /// <summary>
    /// Логика взаимодействия для LazuriteView.xaml
    /// </summary>
    public partial class LazuriteView : UserControl
    {
        public LazuriteView()
        {
            InitializeComponent();

            SizeChanged += (o, e) =>
             {
                 if (iconView.ActualWidth >= 0 && iconView.ActualWidth <= 16)
                     iconView.Icon = Icons.Icon.Lazurite16;
                 else if (iconView.ActualWidth > 16 && iconView.ActualWidth <= 32)
                     iconView.Icon = Icons.Icon.Lazurite32;
                 else if (iconView.ActualWidth > 32 && iconView.ActualWidth <= 64)
                     iconView.Icon = Icons.Icon.Lazurite64;
                 else if (iconView.ActualWidth > 64 && iconView.ActualWidth <= 128)
                     iconView.Icon = Icons.Icon.Lazurite128;
                 else if (iconView.ActualWidth > 128)
                     iconView.Icon = Icons.Icon.LazuriteBig;
             };
        }
    }
}
