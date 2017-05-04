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

namespace LazuriteUI.Windows.Controls
{
    /// <summary>
    /// Логика взаимодействия для Progress.xaml
    /// </summary>
    public partial class Progress : UserControl
    {
        public Progress()
        {
            InitializeComponent();
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
