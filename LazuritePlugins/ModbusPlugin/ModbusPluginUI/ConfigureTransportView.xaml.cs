using LazuriteUI.Windows.Controls;
using NModbusWrapper;
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

namespace ModbusPluginUI
{
    /// <summary>
    /// Логика взаимодействия для ConfigureTransportView.xaml
    /// </summary>
    public partial class ConfigureTransportView : UserControl
    {
        public ConfigureTransportView(IModbusTransport transport)
        {
            InitializeComponent();
            SetTransport(transport);
            lvSelectType.SelectionChanged += (o, e) => {
                if (lvSelectType.GetItems()[0].Selected)
                    SetTransport(new ModbusRtuTransport());
                else SetTransport(new ModbusTcpTransport());
            };

            btApply.Click += (o, e) => ApplyPressed?.Invoke(GetTransport());
        }
        
        private ITransportView _transportView;

        private void SetTransport(IModbusTransport transport)
        {
            if (transport is ModbusRtuTransport)
            {
                _transportView = new ConfigureRtuTransportView();
                lvSelectType.GetItems()[0].Selected = true;
            }
            else
            {
                _transportView = new ConfigureTcpTransportView();
                lvSelectType.GetItems()[1].Selected = true;
            }

            contentControl.Content = _transportView;
            _transportView.Transport = transport;
        }

        public IModbusTransport GetTransport()
        {
            return _transportView.Transport;
        }

        public event Action<IModbusTransport> ApplyPressed;

        public static void Show(Action<IModbusTransport> transportCreated, IModbusTransport transport=null, Grid parent=null)
        {
            if (transport == null)
                transport = new ModbusRtuTransport();

            var control = new ConfigureTransportView(transport);
            var dialog = new DialogView(control);
            control.ApplyPressed += (newTransport) => {
                dialog.Close();
                transportCreated(newTransport);
            };
            dialog.Show(parent);
        }
    }
}
