using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using Lazurite.Shared;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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

namespace UserGeolocationPluginUI
{
    /// <summary>
    /// Логика взаимодействия для UserLocationsView.xaml
    /// </summary>
    public partial class UserLocationsView : System.Windows.Controls.UserControl
    {
        private static readonly string StandardOverlayName = "overlay";

        private GMapControl gmapControl;
        private IGeolocationTarget[] _viewTargets;
        private GeolocationPlace[] _geolocationPlaces;
        private DateTime _viewSince = DateTime.Now.AddDays(-1);

        public UserLocationsView()
        {
            InitializeComponent();

            btAllTime.Click += (o,e) => {
                _viewSince = DateTime.MinValue;
                Refresh();
            };

            btLastDay.Click += (o, e) => {
                _viewSince = DateTime.Now.AddDays(-1);
                Refresh();
            };

            btLastMonth.Click += (o, e) => {
                _viewSince = DateTime.Now.AddMonths(-1);
                Refresh();
            };

            btLastWeek.Click += (o, e) => {
                _viewSince = DateTime.Now.AddDays(-7);
                Refresh();
            };

            btSearch.Click += (o, e) => {
                if (!string.IsNullOrEmpty(tbSearch.Text))
                    gmapControl.SetPositionByKeywords(tbSearch.Text);
            };

            tbSearch.KeyDown += (o, e) => {
                if (e.Key == Key.Enter && !string.IsNullOrEmpty(tbSearch.Text))
                {
                    gmapControl.SetPositionByKeywords(tbSearch.Text);
                    UpdateCurrentCoords();
                }
            };

            wfHost.Child = gmapControl = new GMapControl();
            gmapControl.Bearing = 0;
            gmapControl.MaxZoom = 18;
            gmapControl.MinZoom = 2;
            gmapControl.Zoom = 1;
            gmapControl.MapProvider = GMapProviders.GoogleMap;
            gmapControl.Bearing = 0;
            gmapControl.CanDragMap = true;
            gmapControl.DragButton = MouseButtons.Left;
            gmapControl.MaxZoom = 18;
            gmapControl.MinZoom = 2;
            gmapControl.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionWithoutCenter;
            gmapControl.ShowTileGridLines = false;

            gmapControl.OnMarkerClick += (o, e) => {
                if (o.Tag is UserAndDevice)
                    UserNavigated?.Invoke(this, new EventsArgs<UserAndDevice>((UserAndDevice)o.Tag));
                else if (o.Tag is Place)
                    PlaceNavigated?.Invoke(this, new EventsArgs<string>(((Place)o.Tag).PlaceName));
            };

            gmapControl.OnMapDrag += UpdateCurrentCoords;
            gmapControl.OnMapZoomChanged += UpdateCurrentCoords;
        }
        
        private void UpdateCurrentCoords()
        {
            var center = gmapControl.ViewArea.LocationMiddle;
            tbCurrentLocation.Text =
            center.Lat.ToString().Replace(",", ".") + " " +
            center.Lng.ToString().Replace(",", ".");
        }

        public Geolocation CurrentLocation
        {
            get
            {
                return new Geolocation(gmapControl.ViewArea.LocationMiddle.Lat, gmapControl.ViewArea.LocationMiddle.Lng);
            }
        }

        public void FitToMarkers()
        {
            var rect = gmapControl.GetRectOfAllMarkers(StandardOverlayName);
            if (rect != null)
            {
                gmapControl.SetZoomToFitRect(rect.Value);
                UpdateCurrentCoords();
            }
        }

        public void NavigateTo(string userName, string deviceId)
        {
            var route = GetAllRoutes().FirstOrDefault(x => x.Name == userName + deviceId);
            if (route != null)
            {
                var rect = gmapControl.GetRectOfRoute(route);
                if (rect != null)
                {
                    gmapControl.SetZoomToFitRect(rect.Value);
                    UpdateCurrentCoords();
                }
            }
        }

        public void NavigateTo(GeolocationPlace place)
        {
            gmapControl.SetPositionByKeywords(place.Location.ToString());
            var point = new PointLatLng(place.Location.Latitude, place.Location.Longtitude);
            var pointDelta1 = FindPointAtDistanceFrom(point, 0, place.MetersRadius / 1000);
            var pointDelta2 = FindPointAtDistanceFrom(point, Math.PI, place.MetersRadius / 1000);
            gmapControl.SetZoomToFitRect(new RectLatLng(point, new SizeLatLng((pointDelta1.Lat - pointDelta2.Lat), (pointDelta1.Lng - pointDelta2.Lng))));
            UpdateCurrentCoords();
        }

        private MapRoute[] GetAllRoutes()
        {
            return gmapControl.Overlays.SelectMany(x => x.Routes).ToArray();
        }

        private GMarkerGoogle[] GetAllPlaces()
        {
            return gmapControl.Overlays.SelectMany(x => x.Markers)
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

        public void Refresh()
        {
            gmapControl.Overlays.Clear();
            foreach (var geolocationPlace in _geolocationPlaces)
                CreateCircle(geolocationPlace.Location.Latitude, geolocationPlace.Location.Longtitude, geolocationPlace.MetersRadius, geolocationPlace.Name);
            foreach (var geoTarget in _viewTargets)
                foreach (var device in geoTarget.Geolocations.Select(x=>x.Device).Distinct())
                {
                    var locations = 
                        geoTarget.Geolocations
                        .Where(x => x.Device.Equals(device) && x.DateTime >= _viewSince)
                        .Select(x => new PointDate() {
                            DateTime = x.DateTime,
                            Point = new PointLatLng(x.Geolocation.Latitude, x.Geolocation.Longtitude)
                        })
                        .ToList();
                    CreateRoute(locations, geoTarget.Id, geoTarget.Name, device);
                }
            gmapControl.Zoom--; //crutch
            gmapControl.Zoom++; //crutch
            UpdateCurrentCoords();
        }

        private void CreateRoute(List<PointDate> points, string userId, string userName, string device)
        {
            var overlay = new GMapOverlay("overlay");
            overlay.Routes.Add(new GMapRoute(points.Select(x => x.Point).ToList(), userName+device));
            foreach (var pointDate in points.Take(points.Count-1))
            {
                var marker = new GMarkerGoogle(pointDate.Point, GMarkerGoogleType.blue_small);
                marker.ToolTipText = pointDate.DateTime.ToShortDateString() + " " + pointDate.DateTime.ToShortTimeString();
                marker.ToolTipMode = MarkerTooltipMode.OnMouseOver;
                overlay.Markers.Add(marker);
            }
            if (points.Any())
            {
                var lastPointDate = points.Last();
                var markerEnd = new GMarkerGoogle(lastPointDate.Point, GMarkerGoogleType.blue_dot);
                markerEnd.ToolTipText = string.Format(
                    "Локация пользователя '{0}'\r\nДата: {1}\r\n Устройство: {2}", 
                    userName, 
                    lastPointDate.DateTime.ToShortDateString() + " " + lastPointDate.DateTime.ToShortTimeString(),
                    device);
                markerEnd.Tag = new UserAndDevice(userName, userId, device);
                markerEnd.ToolTipMode = MarkerTooltipMode.Always;
                overlay.Markers.Add(markerEnd);
            }
            gmapControl.Overlays.Add(overlay);
        }

        private void CreateCircle(Double lat, Double lon, double radius, string caption)
        {
            var point = new PointLatLng(lat, lon);
            var segments = 2 * Math.PI;

            var polygonPoints = new List<PointLatLng>();

            for (double i = 0; i < segments; i+=0.1)
                polygonPoints.Add(FindPointAtDistanceFrom(point, i, radius / 1000));

            var gpol = new GMapPolygon(polygonPoints, "circle");
            gpol.Stroke = new System.Drawing.Pen(System.Drawing.Color.Orchid);
            var overlay = new GMapOverlay(StandardOverlayName);
            overlay.Markers.Add(
                new GMarkerGoogle(new PointLatLng(lat, lon), GMarkerGoogleType.green_dot)
                {
                    ToolTipMode = MarkerTooltipMode.Always,
                    ToolTipText = caption,
                    Tag = new Place(caption)
                }
            );
            overlay.Polygons.Add(gpol);
            gmapControl.Overlays.Add(overlay);
        }
        
        private static PointLatLng FindPointAtDistanceFrom(GMap.NET.PointLatLng startPoint, double initialBearingRadians, double distanceKilometres)
        {
            const double radiusEarthKilometres = 6371.01;
            var distRatio = distanceKilometres / radiusEarthKilometres;
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

        private struct PointDate
        {
            public PointLatLng Point { get; set; }
            public DateTime DateTime { get; set; }
        }

        public struct UserAndDevice
        {
            public UserAndDevice(string userName, string userId, string deviceId)
            {
                UserId = userId;
                UserName = userName;
                DeviceId = deviceId;
            }

            public string UserId { get; private set; }
            public string UserName { get; private set; }
            public string DeviceId { get; private set; }
        }

        public struct Place
        {
            public Place(string name)
            {
                PlaceName = name;
            }

            public string PlaceName { get; private set; }
        }
    }
}
