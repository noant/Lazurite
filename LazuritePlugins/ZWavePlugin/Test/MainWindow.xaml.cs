using Lazurite.Data;
using Lazurite.IOC;
using OpenZWrapper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using ZWavePlugin;

namespace Test
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const int OutputPower = 28;
        const int OutputPort = 29;
        const int TransmissionMode = 31;
        const int Endpoint = 38;
        const int TvCode = 27;

        public MainWindow()
        {
            InitializeComponent();
            Singleton.Add(new DataManagerStub());
            ZWavePlugin.ZWaveManager.Current.WaitForInitialized();
            ZWavePlugin.ZWaveManager.Current.AddController(new OpenZWrapper.Controller() { IsHID = false, Path = "COM4" }, (res) => {
                if (res)
                {
                    var b = new ZWaveNodeValue() {
                        HomeId = 4242162579,
                        ValueId = 72057594093240320,
                        NodeId = 3
                    };

                    var t = new Thread(() => {
                        b.Initialize();
                        var codes = new[] { 1181,
                                            2551,
                                            2591,
                                            2791,
                                            2931,
                                            2941,
                                            2951,
                                            2961,
                                            3191,
                                            3331,
                                            3541,
                                            3591,
                                            3601,
                                            3681,
                                            3711,
                                            3731,
                                            3841,
                                            3851,
                                            3881,
                                            3891,
                                            3911,
                                            3921,
                                            4401};
                        while (true)
                        {
                            b.UserInitializeWith(null, true);

                            var node = ZWavePlugin.ZWaveManager.Current.GetNodes().Last();
                            var basic = node.Values.First();
                            SendCommand(node, basic, 3711, 5);
                            //foreach (var code in codes)
                            //{
                            //    SendCommand(node, basic, code);
                            //}
                        }
                    });
                    t.SetApartmentState(ApartmentState.STA);
                    t.Start();
                }
            });
        }

        private void SendCommand(Node node, NodeValue val, int tvcode, int btcode)
        {
            //node.SetConfigParam(TvCode, tvcode);
            //node.SendNodeInformation();
            //Thread.Sleep(20000);
            //node.SetConfigParam(21, 29);
            //node.SetConfigParam(25, 5);
            //node.SetConfigParam(21, 0);
            //node.SetConfigParam(30, 255);
            val.Current = (byte)29;
            //node.SetConfigParam(OutputPort, 1);
            //node.SetConfigParam(OutputPower, 255);
            //node.SetNodeOn();
            //node.SetConfigParam(TransmissionMode, 255);

            Debug.WriteLine("code: " + tvcode + " dt:" + DateTime.Now);
        }

        private void B_ValueChanged(Lazurite.ActionsDomain.IAction action, string value)
        {
            MessageBox.Show(DateTime.Now.ToString());
        }

        private class DataManagerStub : PluginsDataManagerBase
        {
            Dictionary<string, object> _cache = new Dictionary<string, object>();
            public override void Clear(string key)
            {
                _cache.Remove(key);
            }

            public override T Get<T>(string key)
            {
                return (T)_cache[key];
            }

            public override bool Has(string key)
            {
                return _cache.ContainsKey(key);
            }

            public override void Set<T>(string key, T data)
            {
                if (Has(key))
                    _cache[key] = data;
                else _cache.Add(key, data);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}
