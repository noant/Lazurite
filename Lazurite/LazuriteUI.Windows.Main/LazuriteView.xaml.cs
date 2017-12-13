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

            this.SizeChanged += (o, e) =>
             {
                 if (this.iconView.ActualWidth >= 0 && this.iconView.ActualWidth <= 16)
                     this.iconView.Icon = Icons.Icon.Lazurite16;
                 else if (this.iconView.ActualWidth > 16 && this.iconView.ActualWidth <= 32)
                     this.iconView.Icon = Icons.Icon.Lazurite32;
                 else if (this.iconView.ActualWidth > 32 && this.iconView.ActualWidth <= 64)
                     this.iconView.Icon = Icons.Icon.Lazurite64;
                 else if (this.iconView.ActualWidth > 64 && this.iconView.ActualWidth <= 128)
                     this.iconView.Icon = Icons.Icon.Lazurite128;
                 else if (this.iconView.ActualWidth > 128)
                     this.iconView.Icon = Icons.Icon.LazuriteBig;
             };
        }
    }
}
