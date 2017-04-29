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
    /// Логика взаимодействия для Progress.xaml
    /// </summary>
    public partial class Progress : UserControl
    {
        public static DependencyProperty WidthToThicknessProperty = DependencyProperty.Register(nameof(WidthToThickness), typeof(Thickness), typeof(Progress));
        public Progress()
        {
            InitializeComponent();
            this.SizeChanged += (o, e) => 
                WidthToThickness = new Thickness(this.ActualWidth, 0, 0, 0);
            this.Loaded += (o,e) => 
                WidthToThickness = new Thickness(this.ActualWidth, 0, 0, 0);
        }

        public Thickness WidthToThickness
        {
            get
            {
                return (Thickness)GetValue(WidthToThicknessProperty);
            }
            set
            {
                SetValue(WidthToThicknessProperty, value);
            }
        }

        public void StartProgress()
        {
            moveGrid.Visibility = Visibility.Visible;
            ((Storyboard)moveGrid.Resources["moveGridAnimation"]).Begin(moveGrid);
        }
        
        public void StopProgress()
        {
            moveGrid.Visibility = Visibility.Collapsed;
            ((Storyboard)moveGrid.Resources["moveGridAnimation"]).Stop(moveGrid);
        }
    }
}
