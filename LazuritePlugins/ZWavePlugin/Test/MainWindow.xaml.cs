using Lazurite.Data;
using Lazurite.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using ZWavePlugin;

namespace Test
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ZWaveNodeValue B;

        public MainWindow()
        {
            InitializeComponent();
            Singleton.Add(new DataManagerStub());
            var b = new ZWaveNodeValue();
            b.UserInitializeWith(null, false);
            b.ValueChanged += B_ValueChanged;
            B = b;

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
            B.SetValue(null, "TRUE");
        }
    }
}
