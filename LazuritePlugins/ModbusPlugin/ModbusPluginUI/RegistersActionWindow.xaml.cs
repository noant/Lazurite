using NModbusWrapper;
using System.Windows;

namespace ModbusPluginUI
{
    /// <summary>
    /// Логика взаимодействия для SingleCoilActionWindow.xaml
    /// </summary>
    public partial class RegistersActionWindow : Window
    {
        public RegistersActionWindow(IModbusRegistersAction action, ValueTypeMode mode)
        {
            InitializeComponent();
            actionView.RefreshWith(action, mode);
            actionView.OkPressed += (a) => {
                this.DialogResult = true;
            };
            actionView.CancelPressed += () => {
                this.DialogResult = false;
            };
        }
    }
}
