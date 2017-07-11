using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace LazuriteUI.Windows.Controls
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static void InitializeResources()
        {
            if (Initialized)
                return;

            var resources = new ResourceDictionary();
            resources.Source = new Uri(@"/LazuriteUI.Windows.Controls;component/Styles/Styles.xaml", UriKind.RelativeOrAbsolute);
            foreach (var resourceKey in resources.Keys)
            {
                if (!Application.Current.Resources.Contains(resourceKey))
                    Application.Current.Resources.Add(resourceKey, resources[resourceKey]);
            }
            Initialized = true;
        }

        public static bool Initialized { get; private set; }
    }
}
