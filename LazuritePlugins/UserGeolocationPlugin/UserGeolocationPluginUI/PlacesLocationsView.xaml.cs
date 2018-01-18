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
    /// Логика взаимодействия для PlacesLocationsView.xaml
    /// </summary>
    public partial class PlacesLocationsView : UserControl, ILocationsView
    {
        private IGeolocationTarget[] _targets;

        public PlacesLocationsView()
        {
            InitializeComponent();

            bool ignorePlaceNavigation = false;
            placesView.SelectedPlaceChanged += (o, e) =>
            {
                placeEditView.RefreshWith(e.Value);
                if (!ignorePlaceNavigation)
                    locationsView.NavigateTo(e.Value);
                btRemovePlace.IsEnabled = e.Value != null;
                SelectedPlaceChanged?.Invoke(this, new EventsArgs<GeolocationPlace>(e.Value));
            };

            locationsView.PlaceNavigated += (o, e) => 
            {
                ignorePlaceNavigation = true;
                var placeToSelect = placesView.Places.FirstOrDefault(x => x.Name == e.Value);
                placesView.SelectedPlace = placeToSelect;
                btRemovePlace.IsEnabled = placesView.SelectedPlace != null;
                ignorePlaceNavigation = false;
            };

            btAddPlace.Click += (o, e) => 
            {
                var geolocationPlace = new GeolocationPlace();
                geolocationPlace.Name =
                    "Новое место " + (PlacesManager.Current.Places.Count(x => x.Name.StartsWith("Новое место")) + 1);
                geolocationPlace.Location = locationsView.CurrentLocation;
                geolocationPlace.MetersRadious = 200;
                placesView.AddPlace(geolocationPlace);
                RefreshWith(_targets, null);
                placesView.SelectedPlace = geolocationPlace;
            };

            btRemovePlace.Click += (o, e) => {
                if (placesView.SelectedPlace != null)
                {
                    placesView.RemovePlace(placesView.SelectedPlace);
                    RefreshWith(_targets, null);
                }
            };

            placeEditView.SettingsApplied += (o, e) => {
                ignorePlaceNavigation = true;
                RefreshWith(_targets, null);
                placesView.SelectedPlace = e.Value;
                ignorePlaceNavigation = false;
            };
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

        public void RefreshWith(IGeolocationTarget[] viewTargets, GeolocationPlace[] geolocationPlaces)
        {
            _targets = viewTargets;
            var places = placesView.Places;
            if (geolocationPlaces != null)
                places = places.Union(geolocationPlaces).ToArray();
            locationsView.RefreshWith(viewTargets, places);
            placesView.Refresh();
        }

        public void HideDevicesExcept(string device) => locationsView.HideDevicesExcept(device);

        public GeolocationPlace SelectedPlace
        {
            get
            {
                return placesView.SelectedPlace;
            }
            set
            {
                placesView.SelectedPlace = value;
            }
        }

        public event EventsHandler<GeolocationPlace> SelectedPlaceChanged;
    }
}
