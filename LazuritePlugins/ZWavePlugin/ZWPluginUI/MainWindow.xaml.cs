using OpenZWrapper;
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

namespace ZWPluginUI
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            mainView.ButtonApply.Click += (o, e) => this.DialogResult = true;
            mainView.ButtonCancel.Click += (o, e) => this.DialogResult = false;
        }

        public void RefreshWith(ZWaveManager manager, NodeValue selected = null, Func<NodeValue, bool> comparability = null)
        {
            mainView.RefreshWith(manager, selected, comparability);
        }

        public NodeValue GetSelectedNodeValue() => mainView.SelectedNodeValue;
    }
}
