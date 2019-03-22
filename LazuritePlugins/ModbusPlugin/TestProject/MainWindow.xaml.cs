using Lazurite.ActionsDomain.ValueTypes;
using ModbusPlugin;
using System.Windows;

namespace TestProject
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Test();
        }

        public void Test()
        {
            var action = new ModbusSingleCoilAction();
            var result = action.UserInitializeWith(null, false);
        }
    }
}
