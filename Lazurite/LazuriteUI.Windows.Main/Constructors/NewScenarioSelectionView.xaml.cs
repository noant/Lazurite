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

namespace LazuriteUI.Windows.Main.Constructors
{
    /// <summary>
    /// Логика взаимодействия для NewScenarioSelectionView.xaml
    /// </summary>
    public partial class NewScenarioSelectionView : UserControl
    {
        public NewScenarioSelectionView()
        {
            InitializeComponent();
        }

        private void btSingleActionScenario_Click(object sender, RoutedEventArgs e)
        {
            SingleActionScenario?.Invoke();
        }

        private void btComplexScenario_Click(object sender, RoutedEventArgs e)
        {
            CompositeScenario?.Invoke();
        }

        private void btRemoteScenario_Click(object sender, RoutedEventArgs e)
        {
            RemoteScenario?.Invoke();
        }

        public event Action SingleActionScenario;
        public event Action CompositeScenario;
        public event Action RemoteScenario;
    }
}
