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
using System.Windows.Shapes;
using UserGeolocationPlugin;

namespace UserGeolocationPluginUI
{
    /// <summary>
    /// Логика взаимодействия для UserInLocationActionWindow.xaml
    /// </summary>
    public partial class GetDistanceBetweenUserAndLocationActionWindow : Window
    {
        public GetDistanceBetweenUserAndLocationActionWindow()
        {
            InitializeComponent();
            locationsView.SelectedUserChanged += (o, e) => UpdateControls();
            locationsView.SelectedDeviceChanged += (o, e) => UpdateControls();
            locationsView.SelectedPlaceChanged += (o, e) => UpdateControls();
            btContinue.Click += (o, e) => DialogResult = true;
        }

        public string SelectedDevice
        {
            get => locationsView.SelectedDevice;
            set => locationsView.SelectedDevice = value;
        }

        public IGeolocationTarget SelectedUser
        {
            get => locationsView.SelectedUser;
            set => locationsView.SelectedUser = value;
        }

        public GeolocationPlace SelectedPlace
        {
            get => locationsView.SelectedPlace;
            set => locationsView.SelectedPlace = value;
        }

        public IGeolocationTarget[] Users { get; set; }

        public void Refresh() => locationsView.RefreshWith(Users, null);

        public void UpdateControls()
        {
            btContinue.IsEnabled = SelectedPlace != null && SelectedUser != null && SelectedDevice != null;
            tbSelectedParams.Text = 
                string.Format("Выбранный пользователь: {0};\r\nУстройство: {1};\r\nВыбранное место: {2}", 
                    SelectedUser?.Name ?? "[не выбран]", SelectedDevice ?? "[не выбрано]", SelectedPlace?.Name ?? "[не выбрано]");
        }
    }
}
