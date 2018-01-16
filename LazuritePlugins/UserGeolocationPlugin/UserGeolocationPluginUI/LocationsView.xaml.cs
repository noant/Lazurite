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
using Lazurite.Shared;
using UserGeolocationPlugin;

namespace UserGeolocationPluginUI
{
    /// <summary>
    /// Логика взаимодействия для LocationsView.xaml
    /// </summary>
    public partial class LocationsView : Grid, ILocationsView
    {
        public LocationsView()
        {
            InitializeComponent();
            bool ignoreUserNavigation = false;
            usersView.SelectionChanged += (o, e) => {
                devicesView.Devices = usersView.SelectedUser
                    .Geolocations
                    .Select(x => x.Device)
                    .Distinct()
                    .ToArray();
                devicesView.SelectedDevice = devicesView.Devices.FirstOrDefault();
                SelectedUserChanged?.Invoke(this, new EventsArgs<IGeolocationTarget>(SelectedUser));
            };
            devicesView.SelectionChanged += (o, e) => {
                if (!ignoreUserNavigation)
                    locationsView.NavigateTo(SelectedUser.Name, SelectedDevice);
                 SelectedDeviceChanged?.Invoke(this, new EventsArgs<string>(SelectedDevice));
            };
            locationsView.UserNavigated += (o, e) => {
                ignoreUserNavigation = true;
                usersView.SelectedUser = usersView.Users.FirstOrDefault(x => x.Id == e.Value.UserId);
                devicesView.SelectedDevice = e.Value.DeviceId;
                ignoreUserNavigation = false;
            };
            locationsView.SelectedPlaceChanged += (o, e) => SelectedPlaceChanged?.Invoke(this, e);

            this.Loaded += (o, e) => FitToMarkers();
        }

        public Geolocation CurrentLocation => locationsView.CurrentLocation;

        public event EventsHandler<string> PlaceNavigated
        {
            add => locationsView.PlaceNavigated += value;
            remove => locationsView.PlaceNavigated -= value;
        }

        public event EventsHandler<UserAndDevice> UserNavigated
        {
            add => locationsView.UserNavigated += value;
            remove => locationsView.UserNavigated -= value;
        }

        public void FitToMarkers() => locationsView.FitToMarkers();

        public void NavigateTo(string userName, string deviceId) => locationsView.NavigateTo(userName, deviceId);

        public void NavigateTo(GeolocationPlace place) => locationsView.NavigateTo(place);

        public void Refresh() => locationsView.Refresh();

        public GeolocationPlace SelectedPlace
        {
            get => locationsView.SelectedPlace;
            set => locationsView.SelectedPlace = value;
        }

        public IGeolocationTarget SelectedUser
        {
            get => usersView.SelectedUser;
            set => usersView.SelectedUser = value;
        }

        public string SelectedDevice
        {
            get => devicesView.SelectedDevice;
            set => devicesView.SelectedDevice = value;
        }

        public event EventsHandler<IGeolocationTarget> SelectedUserChanged;
        public event EventsHandler<string> SelectedDeviceChanged;
        public event EventsHandler<GeolocationPlace> SelectedPlaceChanged;

        public void RefreshWith(IGeolocationTarget[] viewTargets, GeolocationPlace[] geolocationPlaces)
        {
            usersView.Users = viewTargets;
            locationsView.RefreshWith(viewTargets, geolocationPlaces);
        }
    }
}
