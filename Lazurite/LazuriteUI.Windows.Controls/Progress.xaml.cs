using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

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
