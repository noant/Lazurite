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
        private GMapControl gmapControl;
        private IGeolocationTarget[] _viewTargets;
        private GeolocationPlace[] _geolocationPlaces;

        public UserLocationsView()
        {
            InitializeComponent();

            wfHost.Child = gmapControl = new GMapControl();

            gmapControl.MapProvider = GMapProviders.GoogleMap;
            gmapControl.Bearing = 0;
            gmapControl.CanDragMap = true;
            gmapControl.DragButton = MouseButtons.Left;
            gmapControl.MaxZoom = 18;
            gmapControl.MinZoom = 2;
            gmapControl.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionWithoutCenter;
            gmapControl.ShowTileGridLines = false;
            gmapControl.Click += GmapControl_Click;
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
                        geoTarget.Geolocations.Where(x => x.Device.Equals(device))
                        .Select(x=> new PointDate() {
                            DateTime = x.DateTime,
                            Point = new PointLatLng(x.Geolocation.Latitude, x.Geolocation.Longtitude)
                        })
                        .ToList();
                    CreateRoute(locations, geoTarget.Name, device);
                }
        }

        private void CreateRoute(List<PointDate> points, string userName, string device)
        {
            var overlay = new GMapOverlay();
            overlay.Routes.Add(new GMapRoute(points.Select(x => x.Point).ToList(), "path"));
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
                markerEnd.ToolTipMode = MarkerTooltipMode.Always;
                overlay.Markers.Add(markerEnd);
            }
            gmapControl.Overlays.Add(overlay);
        }

        private void CreateCircle(Double lat, Double lon, double radius, string caption)
        {
            var point = new PointLatLng(lat, lon);
            var segments = 2 * Math.PI;

            var poligonPoints = new List<PointLatLng>();

            for (double i = 0; i < segments; i+=0.1)
                poligonPoints.Add(FindPointAtDistanceFrom(point, i, radius / 1000));

            var gpol = new GMapPolygon(poligonPoints, "circle");
            gpol.Stroke = new System.Drawing.Pen(System.Drawing.Color.Orchid);
            var overlay = new GMapOverlay();
            overlay.Markers.Add(
                new GMarkerGoogle(new PointLatLng(lat, lon), GMarkerGoogleType.green_dot)
                {
                    ToolTipMode = MarkerTooltipMode.Always,
                    ToolTipText = caption
                }
            );
            overlay.Polygons.Add(gpol);
            gmapControl.Overlays.Add(overlay);
        }

        private void GmapControl_Click(object sender, EventArgs e)
        {
            gmapControl.SetPositionByKeywords("55.715045, 37.828406");
            var point = new PointLatLng(55.715045, 37.828406);
            var pointDelta1 = FindPointAtDistanceFrom(point, 0, 2);
            var pointDelta2 = FindPointAtDistanceFrom(point, Math.PI, 2);
            gmapControl.SetZoomToFitRect(new RectLatLng(point, new SizeLatLng((pointDelta1.Lat - pointDelta2.Lat), (pointDelta1.Lng - pointDelta2.Lng))));
        }

        public static GMap.NET.PointLatLng FindPointAtDistanceFrom(GMap.NET.PointLatLng startPoint, double initialBearingRadians, double distanceKilometres)
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

        public static double DegreesToRadians(double degrees)
        {
            const double degToRadFactor = Math.PI / 180;
            return degrees * degToRadFactor;
        }

        public static double RadiansToDegrees(double radians)
        {
            const double radToDegFactor = 180 / Math.PI;
            return radians * radToDegFactor;
        }

        private struct PointDate
        {
            public PointLatLng Point { get; set; }
            public DateTime DateTime { get; set; }
        }
    }
}
