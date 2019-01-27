using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.IOC;
using Lazurite.Logging;
using Lazurite.MainDomain;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace LazuriteUI.Windows.Main.Switches
{
    /// <summary>
    /// Логика взаимодействия для ToggleView.xaml
    /// </summary>
    public partial class GeolocationView : UserControl
    {
        private static readonly ILogger Log = Singleton.Resolve<ILogger>();

        public GeolocationView()
        {
            InitializeComponent();
        }

        public GeolocationView(ScenarioBase scenario): this()
        {
            var model = new ScenarioModel(scenario);
            DataContext = model;
            Unloaded += (o, e) => model.Dispose();
            itemView.Click += ItemView_Click;
        }

        private void ItemView_Click(object sender, RoutedEventArgs e)
        {
            //open through yandex maps
            var browserUrl = @"https://yandex.ru/maps/?mode=whatshere&whatshere%5Bpoint%5D={0}%2C{1}&whatshere%5Bzoom%5D=13";
            var data = GeolocationData.FromString(((ScenarioModel)DataContext).ScenarioValue);
            var lat = data.Latitude.ToString().Replace(",", ".");
            var lng = data.Longtitude.ToString().Replace(",", ".");

            var url = string.Format(browserUrl, lng, lat);

            try
            {
                Process.Start(url);
            }
            catch
            {
                Process.Start("IEXPLORE.EXE", url); //crutch
            }
        }
    }
}
