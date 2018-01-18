using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using Lazurite.Shared;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UserGeolocationPlugin;
using UserGeolocationPluginUI.Properties;

namespace UserGeolocationPluginUI
{
    /// <summary>
    /// Логика взаимодействия для UserLocationsView.xaml
    /// </summary>
    public partial class UserLocationsView : System.Windows.Controls.UserControl, ILocationsView
    {
        private static readonly string StandardOverlayName = "overlay";

        private GMapControl _gmapControl;
        private IGeolocationTarget[] _viewTargets;
        private GeolocationPlace[] _geolocationPlaces;
        private DateTime _viewSince = DateTime.Now.AddDays(-1);
        private MarkersEnumerator _markersEnumerator;
        private string _currentDevice;

        public UserLocationsView()
        {
            InitializeComponent();

            dpDataSince.SelectedDate = _viewSince = DateTime.Now.AddDays(-1);

            dpDataSince.SelectedDateChanged += (o, e) => {
                _viewSince = dpDataSince.SelectedDate ?? DateTime.MinValue;
                Refresh();
            };
            
            btSearch.Click += (o, e) => {
                if (!string.IsNullOrEmpty(tbSearch.Text))
                    _gmapControl.SetPositionByKeywords(tbSearch.Text);
            };

            tbSearch.KeyDown += (o, e) => {
                if (e.Key == Key.Enter && !string.IsNullOrEmpty(tbSearch.Text))
                {
                    _gmapControl.SetPositionByKeywords(tbSearch.Text);
                    UpdateCurrentCoords();
                }
            };
            
            wfHost.Child = _gmapControl = new GMapControl();
            _gmapControl.Bearing = 0;
            _gmapControl.MaxZoom = 18;
            _gmapControl.MinZoom = 2;
            _gmapControl.Zoom = 1;
            _gmapControl.MapProvider = GMapProviders.GoogleMap;
            _gmapControl.Bearing = 0;
            _gmapControl.CanDragMap = true;
            _gmapControl.DragButton = MouseButtons.Left;
            _gmapControl.MaxZoom = 18;
            _gmapControl.MinZoom = 2;
            _gmapControl.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionWithoutCenter;
            _gmapControl.ShowTileGridLines = false;

            _gmapControl.OnMarkerClick += (o, e) => {
                if (o.Tag is UserAndDevice)
                    UserNavigated?.Invoke(this, new EventsArgs<UserAndDevice>((UserAndDevice)o.Tag));
                else if (o.Tag is Place)
                    PlaceNavigated?.Invoke(this, new EventsArgs<string>(((Place)o.Tag).PlaceName));
            };

            _gmapControl.OnMapDrag += UpdateCurrentCoords;
            _gmapControl.OnMapZoomChanged += UpdateCurrentCoords;
        }
        
        private void UpdateCurrentCoords()
        {
            tbCurrentLocation.Text = CurrentLocation.ToString();
        }

        public Geolocation CurrentLocation => new Geolocation(_gmapControl.Position.Lat, _gmapControl.Position.Lng, false);

        public void FitToMarkers()
        {
            var rect = _gmapControl.GetRectOfAllMarkers(StandardOverlayName);
            if (rect != null)
            {
                _gmapControl.SetZoomToFitRect(rect.Value);
                UpdateCurrentCoords();
            }
        }

        public void NavigateTo(string userName, string deviceId)
        {
            var route = GetAllRoutes().FirstOrDefault(x => x.Name == userName + deviceId);
            if (route != null)
            {
                var rect = _gmapControl.GetRectOfRoute(route);
                if (rect != null)
                {
                    _gmapControl.SetZoomToFitRect(rect.Value);
                    UpdateCurrentCoords();
                }
            }
        }

        public void NavigateTo(GeolocationPlace place)
        {
            if (place != null && !this.CurrentLocation.Equals(place.Location))
            {
                _gmapControl.SuspendLayout();
                var fakeRoute = _gmapControl.Overlays
                    .SelectMany(x => x.Routes)
                    .Where(x => x.Tag is Place)
                    .FirstOrDefault(x => ((Place)x.Tag).PlaceName.Equals(place.Name));
                if (fakeRoute != null)
                {
                    var rectToZoom = _gmapControl.GetRectOfRoute(fakeRoute);
                    if (rectToZoom != null)
                        _gmapControl.SetZoomToFitRect(rectToZoom.Value);
                }
                _gmapControl.Zoom-=2;
                _gmapControl.ResumeLayout();

                UpdateCurrentCoords();
            }
        }

        private MapRoute[] GetAllRoutes()
        {
            return _gmapControl.Overlays.SelectMany(x => x.Routes).ToArray();
        }

        private GMarkerGoogle[] GetAllPlaces()
        {
            return _gmapControl.Overlays.SelectMany(x => x.Markers)
                .Where(x => x is GMarkerGoogle)
                .Where(x => ((GMarkerGoogle)x).Type == GMarkerGoogleType.green_dot)
                .Cast<GMarkerGoogle>()
                .ToArray();
        }

        public void RefreshWith(IGeolocationTarget[] viewTargets, GeolocationPlace[] geolocationPlaces)
        {
            _viewTargets = viewTargets;
            _geolocationPlaces = geolocationPlaces;
            Refresh();
        }
        
        public void HideDevicesExcept(string device)
        {
            if (device != _currentDevice)
            {
                _currentDevice = device;
                Refresh();
            }
        }

        public void Refresh()
        {
            _markersEnumerator = new MarkersEnumerator();

            _gmapControl.SuspendLayout();
            _gmapControl.Overlays.Clear();
            if (_geolocationPlaces != null)
                foreach (var geolocationPlace in _geolocationPlaces)
                    CreateCircle(geolocationPlace.Location.Latitude, geolocationPlace.Location.Longtitude, geolocationPlace.MetersRadious, geolocationPlace.Name);
            if (_viewTargets != null)
                foreach (var geoTarget in _viewTargets)
                    foreach (var device in geoTarget.Geolocations.Select(x=>x.Device).Distinct())
                    {
                        if (string.IsNullOrEmpty(_currentDevice) || device.Equals(_currentDevice))
                        {
                            var geolocations = geoTarget.Geolocations
                                .Where(x => x.Device.Equals(device) && x.DateTime >= _viewSince).ToArray();
                            geolocations = geolocations
                                .Where(x => x.Geolocation.IsGPS || x == geolocations[0])
                                .ToArray();
                            var locations =
                                geolocations
                                .Where(x => x.Device.Equals(device) && x.DateTime >= _viewSince)
                                .Select(x => new PointDate() {
                                    DateTime = x.DateTime,
                                    Point = new PointLatLng(x.Geolocation.Latitude, x.Geolocation.Longtitude)
                                })
                                .ToList();
                            CreateRoute(locations, geoTarget.Id, geoTarget.Name, device);
                        }
                    }
            _gmapControl.ResumeLayout();
            UpdateCurrentCoords();
        }
                
        private void CreateRoute(List<PointDate> points, string userId, string userName, string device)
        {
            var style = _markersEnumerator.Next;

            var overlay = new GMapOverlay("overlay");
            _gmapControl.Overlays.Add(overlay);
            overlay.Routes.Add(
                new GMapRoute(points.Select(x => x.Point).ToList(), userName + device) {
                    Stroke = style.Stroke
                }
            );
            foreach (var pointDate in points.Take(points.Count-1))
            {
                var marker = new GMarkerGoogle(pointDate.Point, style.SmallMarker);
                marker.ToolTipText = marker.ToolTipText = string.Format(
                    "Локация пользователя '{0}'\r\nДата: {1}\r\n Устройство: {2}",
                    userName,
                    pointDate.DateTime.ToShortDateString() + " " + pointDate.DateTime.ToShortTimeString(),
                    device);
                marker.ToolTip.Font = new Font("Calibri", 9);
                marker.ToolTipMode = MarkerTooltipMode.OnMouseOver;
                overlay.Markers.Add(marker);
            }
            if (points.Any())
            {
                var lastPointDate = points.Last();
                var markerEnd = new GMarkerGoogle(lastPointDate.Point, style.BigMarker);
                markerEnd.ToolTipText = string.Format(
                    "Текущая локация пользователя '{0}'\r\nДата: {1}\r\n Устройство: {2}", 
                    userName, 
                    lastPointDate.DateTime.ToShortDateString() + " " + lastPointDate.DateTime.ToShortTimeString(),
                    device);
                markerEnd.ToolTip.Font = new Font("Calibri", 9);
                markerEnd.Tag = new UserAndDevice(userName, userId, device);
                markerEnd.ToolTipMode = MarkerTooltipMode.Always;
                overlay.Markers.Add(markerEnd);
            }
        }

        private void CreateCircle(Double lat, Double lon, double Radious, string caption)
        {
            var point = new PointLatLng(lat, lon);
            var segments = 2 * Math.PI;

            var polygonPoints = new List<PointLatLng>();

            for (double i = 0; i < segments; i+=0.1)
                polygonPoints.Add(FindPointAtDistanceFrom(point, i, Radious / 1000));

            var fakeRoute = new GMapRoute(polygonPoints, "fake");
            fakeRoute.Tag = new Place(caption);
            fakeRoute.Stroke = new System.Drawing.Pen(System.Drawing.Color.Transparent);
            var gpol = new GMapPolygon(polygonPoints, "circle");
            gpol.Stroke = new System.Drawing.Pen(System.Drawing.Color.Orchid);
            var overlay = new GMapOverlay(StandardOverlayName);
            _gmapControl.Overlays.Add(overlay);
            overlay.Markers.Add(
                new GMarkerGoogle(new PointLatLng(lat, lon), GMarkerGoogleType.yellow_pushpin)
                {
                    ToolTipMode = MarkerTooltipMode.Always,
                    ToolTipText = caption,
                    ToolTip = {
                        Font = new Font("Calibri", 9)
                    },
                    Tag = new Place(caption)
                }
            );
            overlay.Routes.Add(fakeRoute);
            overlay.Polygons.Add(gpol);
        }
        
        private static PointLatLng FindPointAtDistanceFrom(GMap.NET.PointLatLng startPoint, double initialBearingRadians, double distanceKilometres)
        {
            const double RadiousEarthKilometres = 6371.01;
            var distRatio = distanceKilometres / RadiousEarthKilometres;
            var distRatioSine = Math.Sin(distRatio);
            var distRatioCosine = Math.Cos(distRatio);

            var startLatRad = DegreesToRadians(startPoint.Lat);
            var startLonRad = DegreesToRadians(startPoint.Lng);

            var startLatCos = Math.Cos(startLatRad);
            var startLatSin = Math.Sin(startLatRad);

            var endLatRads = Math.Asin((startLatSin * distRatioCosine) + (startLatCos * distRatioSine * Math.Cos(initialBearingRadians)));

            var endLonRads = startLonRad + Math.Atan2(
                          Math.Sin(initialBearingRadians) * distRatioSine * startLatCos,
                          distRatioCosine - startLatSin * Math.Sin(endLatRads));

            return new GMap.NET.PointLatLng(RadiansToDegrees(endLatRads), RadiansToDegrees(endLonRads));
        }

        private static double DegreesToRadians(double degrees)
        {
            const double degToRadFactor = Math.PI / 180;
            return degrees * degToRadFactor;
        }

        private static double RadiansToDegrees(double radians)
        {
            const double radToDegFactor = 180 / Math.PI;
            return radians * radToDegFactor;
        }

        public event EventsHandler<string> PlaceNavigated;

        public event EventsHandler<UserAndDevice> UserNavigated;
    }
}
