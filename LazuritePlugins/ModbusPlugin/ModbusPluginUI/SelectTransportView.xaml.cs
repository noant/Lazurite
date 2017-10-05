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

            this.Loaded += (o, e) => {
                var window = Window.GetWindow(this);
                var grid = window.Content as Grid;
                btEdit.Click += (o1, e1) => 
                    ConfigureTransportView.Show(
                        (transport) =>
                        {
                            RefreshWith(transport);
                            TransportChanged?.Invoke(transport);
                        },
                        Transport,
                        grid);
            };
        }
        
        public IModbusTransport Transport { get; private set; }

        public void RefreshWith(IModbusTransport transport)
        {
            Transport = transport;
            lblDescription.Content = transport.ToString();
        }

        public event Action<IModbusTransport> TransportChanged;
    }
}
