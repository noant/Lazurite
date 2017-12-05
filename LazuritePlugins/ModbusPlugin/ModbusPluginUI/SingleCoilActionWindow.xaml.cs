using NModbusWrapper;
using System.Windows;

namespace ModbusPluginUI
{
    /// <summary>
    /// Логика взаимодействия для SingleCoilActionWindow.xaml
    /// </summary>
    public partial class SingleCoilActionWindow : Window
    {
        public SingleCoilActionWindow(IModbusSingleCoilAction action)
        {
            InitializeComponent();
            actionView.RefreshWith(action);
            actionView.OkPressed += (a) => {
                this.DialogResult = true;
            };
            actionView.CancelPressed += () => {
                this.DialogResult = false;
            };
        }
    }
}
