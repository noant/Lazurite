using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.MainDomain;
using LazuriteUI.Windows.Controls;
using System;
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
        public GeolocationView()
        {
            InitializeComponent();
        }

        public GeolocationView(ScenarioBase scenario, UserVisualSettings visualSettings): this()
        {
            this.DataContext = new ScenarioModel(scenario, visualSettings);
            itemView.Click += ItemView_Click;
        }

        private void ItemView_Click(object sender, RoutedEventArgs e)
        {
            //open through yandex maps
            var browserUrl = @"https://yandex.ru/maps/?mode=whatshere&whatshere%5Bpoint%5D={0}%2C{1}&whatshere%5Bzoom%5D=13";
            var data = GeolocationData.FromString(((ScenarioModel)DataContext).ScenarioValue);
            var lat = data.Latitude.ToString().Replace(",", ".");
            var lng = data.Longtitude.ToString().Replace(",", ".");
            Process.Start(string.Format(browserUrl, lng, lat));
        }
    }
}
