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
            //ModbusPluginUI.ConfigureTransportView.Show((t) => {
            //    var transport = t;
            //    t = null;
            //    Test();
            //});

            var action = new ModbusRegistersAction();
            var result = action.UserInitializeWith(new FloatValueType(), false);
            result = action.UserInitializeWith(new InfoValueType(), false);
            result = action.UserInitializeWith(null, false);
            var a = 0;
        }
    }
}
