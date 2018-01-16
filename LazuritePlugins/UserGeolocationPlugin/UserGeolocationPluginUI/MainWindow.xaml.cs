using Lazurite.IOC;
using Lazurite.Shared;
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
using UserGeolocationPlugin;

namespace UserGeolocationPluginUI
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
        }
    }

    public class UserTest : IGeolocationTarget
    {
        public string Name { get; set; }

        public string Id { get; set; } = "123";

        public GeolocationInfo[] Geolocations { get; set; }
    }

    public class TestStorage : Lazurite.Data.PluginsDataManagerBase
    {
        public Dictionary<string, object> _dict = new Dictionary<string, object>();

        public override void Clear(string key)
        {
            _dict.Remove(key);
        }

        public override T Get<T>(string key)
        {
            return (T)_dict[key];
        }

        public override bool Has(string key)
        {
            return _dict.ContainsKey(key);
        }

        public override void Set<T>(string key, T data)
        {
            if (Has(key))
                _dict[key] = data;
            else _dict.Add(key, data);
        }
    }
}
