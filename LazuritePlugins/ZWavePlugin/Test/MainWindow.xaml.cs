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
                            var val = ZWavePlugin.ZWaveManager.Current.GetNodes()
                                .FirstOrDefault(x => x.Id == b.NodeId)
                                .Values.FirstOrDefault(x => x.Id == b.ValueId);

                            var node = val.Node;

                            //node.SetConfigParam(38, 2);
                            //Thread.Sleep(5000);
                            //Thread.Sleep(1500);
                            //node.SetConfigParam(29, 2);
                            //Debug.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                            //Thread.Sleep(9000);
                            //node.SetConfigParam(38, 3);
                            //Thread.Sleep(1500);
                            //node.SetConfigParam(29, 3);
                            //Debug.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                            //Thread.Sleep(9000);
                            //node.SetConfigParam(38, 4);
                            //Thread.Sleep(1500);
                            //node.SetConfigParam(29, 4);
                            //Debug.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                            //Thread.Sleep(9000);
                            //node.SetConfigParam(38, 5);
                            //Thread.Sleep(1500);
                            //node.SetConfigParam(29, 5);
                            //Debug.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                            //Thread.Sleep(9000);
                            //node.SetConfigParam(38, 6);
                            //Thread.Sleep(1500);
                            //node.SetConfigParam(29, 6);
                            //Debug.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                            //Thread.Sleep(9000);

                            //var val2 = ZWavePlugin.ZWaveManager.Current.GetNodes()
                            //    .FirstOrDefault(x => x.Id == b.NodeId)
                            //    .Values.FirstOrDefault(x => x.Name.StartsWith("IR Code num")).Current = (short)3711;

                            //Thread.Sleep(5000);

                            //for (int i = 1; i <= 6; i++)
                            //{
                            //    node.SetConfigParam(38, i);
                            //    Thread.Sleep(3000);
                            //    node.SetConfigParam(29, i);
                            //    Thread.Sleep(3000);
                            //}
                            //for (int i = 0; i<=10;i++)
                            //val.Current = (short)39;

                            var valueLearning = node.Values.FirstOrDefault(x => x.Name.StartsWith("IR Code Lear"));
                            var valueEndPoint = node.Values.FirstOrDefault(x => x.Name.StartsWith("End-point selection"));
                            var valueStatus = node.Values.FirstOrDefault(x => x.Name.StartsWith("Learning status register"));
                            var valueOutput1 = node.Values.FirstOrDefault(x => x.Name.StartsWith("OutputMain"));

                            //valueEndPoint.Current = (byte)1;

                            //Thread.Sleep(5000);
                            //MessageBox.Show("now");
                            //valueLearning.Current = (byte)26;

                            //Thread.Sleep(5000);

                            //MessageBox.Show(valueStatus.Current.ToString());

                            valueOutput1.Current = (short)39;

                            //node.RequestConfigParam(27);
                            //node.SetConfigParam(27, 3711);
                            //var basic = node.GetNodeBasic();
                            //var specific = node.GetNodeSpecific();
                            //var supports = node.SupportsCommandClass(148);
                            //MessageBox.Show("now_AV!");
                            //node.Values.FirstOrDefault(x => x.Name == "AV").Current = (short)39;
                            //Thread.Sleep(4000);
                            //MessageBox.Show("now_AV_1!");
                            //node.Values.FirstOrDefault(x => x.Name == "AV_1").Current = (short)39;
                            //Thread.Sleep(4000);
                            //MessageBox.Show("now_AV_2!");
                            //node.Values.FirstOrDefault(x => x.Name == "AV_2").Current = (short)39;
                            Thread.Sleep(4000);
                        }
                    });
                    t.SetApartmentState(ApartmentState.STA);
                    t.Start();
                }
            });
        }

        private void SendCommand(Node node, NodeValue val, int tvcode, int btcode)
        {
            //node.RequestConfigParam(21);
            //node.RequestConfigParam(22);
            //node.RequestConfigParam(25);
            //node.RequestConfigParam(26);
            //node.RequestConfigParam(27);
            //node.RequestConfigParam(28);
            //node.RequestConfigParam(29);
            //node.RequestConfigParam(30);
            //node.RequestConfigParam(31);
            //node.RequestConfigParam(38);

            //node.RequestConfigParam(23);
            //node.RequestConfigParam(24);

            //node.RequestConfigParam(148);


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
