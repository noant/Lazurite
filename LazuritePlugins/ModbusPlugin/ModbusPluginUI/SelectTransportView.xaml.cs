using NModbusWrapper;
using System;
using System.Windows;
using System.Windows.Controls;

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
