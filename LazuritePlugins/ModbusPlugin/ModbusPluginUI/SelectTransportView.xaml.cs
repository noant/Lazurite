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
    /// Логика взаимодействия для SelectTransportView.xaml
    /// </summary>
    public partial class SelectTransportView : UserControl
    {
        public SelectTransportView()
        {
            InitializeComponent();
            btEdit.Click += (o, e) => 
                ConfigureTransportView.Show(
                    (transport) =>
                    {
                        RefreshWith(transport);
                        TransportChanged?.Invoke(transport);
                    },
                    Transport);
        }

        public IModbusTransport Transport { get; private set; }

        public void RefreshWith(IModbusTransport transport)
        {
            Transport = transport;
            lblDescription.Content = transport.ToString();
        }

        public Action<IModbusTransport> TransportChanged;
    }
}
