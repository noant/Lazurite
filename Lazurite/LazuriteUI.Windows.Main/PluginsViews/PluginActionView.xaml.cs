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

namespace LazuriteUI.Windows.Main.PluginsViews
{
    /// <summary>
    /// Логика взаимодействия для PluginActionView.xaml
    /// </summary>
    public partial class PluginActionView : UserControl
    {
        public PluginActionView(Type actionType)
        {
            InitializeComponent();
            this.DataContext = new PluginActionViewModel(actionType);
        }
    }
}
