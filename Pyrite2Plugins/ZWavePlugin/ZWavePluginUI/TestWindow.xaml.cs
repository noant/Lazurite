using OpenZWrapper;
using PyriteUI.Controls;
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
using System.Windows.Shapes;

namespace ZWavePluginUI
{
    /// <summary>
    /// Логика взаимодействия для TestWindow.xaml
    /// </summary>
    public partial class TestWindow : Window
    {
        public TestWindow()
        {
            InitializeComponent();

            this.Loaded += TestWindow_Loaded;
        }

        private void TestWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var manager = new ZWaveManager();
            manager.ManagerInitialized += (o, e1) =>
            {
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    ctrl.InitializeWith(manager);
                }));
            };
            manager.Initialize();
        }
    }
}
