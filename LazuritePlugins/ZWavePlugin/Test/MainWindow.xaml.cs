using Lazurite.Data;
using Lazurite.IOC;
using System;
using System.Collections.Generic;
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
            var b = new ZWaveNodeValue();
            b.UserInitializeWith(null, false);
            b.ValueChanged += B_ValueChanged;
            b.UserInitializeWith(b.ValueType, false);
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
