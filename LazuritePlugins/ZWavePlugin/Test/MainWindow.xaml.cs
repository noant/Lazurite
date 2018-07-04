using Lazurite.Data;
using Lazurite.IOC;
using System;
using System.Collections.Generic;
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
        public MainWindow()
        {
            InitializeComponent();
            Singleton.Add(new DataManagerStub());
            ZWaveManager.Current.WaitForInitialized();
            ZWaveManager.Current.AddController(new OpenZWrapper.Controller() { IsHID = false, Path = "COM4" }, (res) => {
                if (res)
                {
                    var b = new ZWaveNodeValue() {
                        HomeId = 4242162579,
                        ValueId = 72057594093240320,
                        NodeId = 3
                    };

                    var t = new Thread(() => {
                        b.Initialize();
                        while (true)
                            b.UserInitializeWith(null, false);
                    });
                    t.SetApartmentState(ApartmentState.STA);
                    t.Start();
                }
            });
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
