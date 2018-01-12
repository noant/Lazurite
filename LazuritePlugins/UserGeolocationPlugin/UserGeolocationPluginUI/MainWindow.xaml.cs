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
            InitializeComponent();

            var testUser1 = new UserTest();
            testUser1.Name = "Пользователь 1";
            testUser1.Geolocations = new[] {
                new GeolocationInfo(new Geolocation(55.806588, 37.694551), "huawei nova 2i") { DateTime = new DateTime(2017, 12, 27, 12, 15,0) },
                new GeolocationInfo(new Geolocation(55.770220, 37.743989), "huawei nova 2i") { DateTime = new DateTime(2017, 12, 27, 15, 15,0) },
                new GeolocationInfo(new Geolocation(55.754734, 37.759095), "huawei nova 2i") { DateTime = new DateTime(2017, 12, 27, 18, 15,0) },
                new GeolocationInfo(new Geolocation(55.738468, 37.774202), "huawei nova 2i") { DateTime = new DateTime(2017, 12, 27, 20, 15,0) },
                new GeolocationInfo(new Geolocation(55.719093, 37.789308), "huawei nova 2i") { DateTime = new DateTime(2017, 12, 28, 2, 15,0) },
                new GeolocationInfo(new Geolocation(55.707464, 37.798921), "huawei nova 2i") { DateTime = new DateTime(2017, 12, 28, 3, 15,0) },
                new GeolocationInfo(new Geolocation(55.709790, 37.819520), "huawei nova 2i") { DateTime = new DateTime(2017, 12, 28, 5, 15,0) },

                new GeolocationInfo(new Geolocation(55.818187, 37.432252), "xiaomi") { DateTime = new DateTime(2017, 12, 27, 12, 15,0) },
                new GeolocationInfo(new Geolocation(55.806588, 37.452851), "xiaomi") { DateTime = new DateTime(2017, 12, 27, 15, 15,0) },
                new GeolocationInfo(new Geolocation(55.800400, 37.478944), "xiaomi") { DateTime = new DateTime(2017, 12, 27, 18, 15,0) },
                new GeolocationInfo(new Geolocation(55.789569, 37.510530), "xiaomi") { DateTime = new DateTime(2017, 12, 27, 20, 15,0) },
                new GeolocationInfo(new Geolocation(55.779509, 37.527009), "xiaomi") { DateTime = new DateTime(2017, 12, 28, 2, 15,0) },
                new GeolocationInfo(new Geolocation(55.764027, 37.553102), "xiaomi") { DateTime = new DateTime(2017, 12, 28, 3, 15,0) },
                new GeolocationInfo(new Geolocation(55.750862, 37.588807), "xiaomi") { DateTime = new DateTime(2017, 12, 28, 5, 15,0) }
            };

            locationsView.RefreshWith(new[] { testUser1 }, new[] {
                new GeolocationPlace() {
                    Location = new Lazurite.Shared.Geolocation(55.715045, 37.828406),
                    MetersRadius = 2000,
                    Name = "Дом"
                },
                new GeolocationPlace() {
                    Location = new Lazurite.Shared.Geolocation(55.754734, 37.620393),
                    MetersRadius = 3000,
                    Name = "Центр москвы"
                },
            });
        }
    }

    public class UserTest : IGeolocationTarget
    {
        public string Name { get; set; }

        public string Id => throw new NotImplementedException();

        public GeolocationInfo[] Geolocations { get; set; }
    }
}
