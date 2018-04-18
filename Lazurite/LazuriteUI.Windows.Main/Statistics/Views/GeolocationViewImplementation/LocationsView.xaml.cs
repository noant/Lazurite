using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using Lazurite.ActionsDomain.ValueTypes;
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

namespace LazuriteUI.Windows.Main.Statistics.Views.GeolocationViewImplementation
{
    /// <summary>
    /// Логика взаимодействия для LocationsView.xaml
    /// </summary>
    public partial class LocationsView : System.Windows.Controls.UserControl
    {
        private static readonly string StandardOverlayName = "overlay";

        private GMapControl _gmapControl;
        private GeolocationScenarioHistoryView[] _viewTargets;
        private DateTime _viewSince = DateTime.Now.AddDays(-1);
        private MarkersEnumerator _markersEnumerator;
        private string _currentDevice;

        public LocationsView()
        {
            InitializeComponent();
                        
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
                if (o.Tag is ScenarioInfo)
                    Navigated?.Invoke(this, new EventsArgs<ScenarioInfo>((ScenarioInfo)o.Tag));
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

        public void NavigateTo(string scenarioId)
        {
            var route = GetAllRoutes().FirstOrDefault(x => x.Name == scenarioId);
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

        public void RefreshWith(GeolocationScenarioHistoryView[] views)
        {
            _viewTargets = views;
            Refresh();
        }
        
        public void Refresh()
        {
            _markersEnumerator = new MarkersEnumerator();

            _gmapControl.SuspendLayout();
            _gmapControl.Overlays.Clear();

            if (_viewTargets != null)
                foreach (var target in _viewTargets)
                {
                    var points = target.Datas.Select(x => new PointDate() {
                        DateTime = x.DateTime,
                        Point = new PointLatLng(x.Latitude, x.Longtitude)
                    }).ToArray();
                    CreateRoute(points, target.ScenarioInfo.Id, target.ScenarioInfo.Name);
                }

            _gmapControl.ResumeLayout();
            UpdateCurrentCoords();
        }
                
        private void CreateRoute(PointDate[] points, string scenarioId, string scenarioName)
        {
            var style = _markersEnumerator.Next;

            var overlay = new GMapOverlay("overlay");
            _gmapControl.Overlays.Add(overlay);
            overlay.Routes.Add(
                new GMapRoute(points.Select(x => x.Point).ToList(), scenarioId) {
                    Stroke = style.Stroke
                }
            );
            foreach (var pointDate in points.Take(points.Length - 1))
            {
                var marker = new GMarkerGoogle(pointDate.Point, style.SmallMarker);
                marker.ToolTipText = marker.ToolTipText = string.Format(
                    "{0}\r\nДата: {1}",
                    scenarioName,
                    pointDate.DateTime.ToShortDateString() + " " + pointDate.DateTime.ToShortTimeString());
                marker.ToolTip.Font = new Font("Calibri", 9);
                marker.ToolTipMode = MarkerTooltipMode.OnMouseOver;
                overlay.Markers.Add(marker);
            }
            if (points.Any())
            {
                var lastPointDate = points.Last();
                var markerEnd = new GMarkerGoogle(lastPointDate.Point, style.BigMarker);
                markerEnd.ToolTipText = string.Format(
                    "{0}\r\nДата: {1}",
                    scenarioName, 
                    lastPointDate.DateTime.ToShortDateString() + " " + lastPointDate.DateTime.ToShortTimeString());
                markerEnd.ToolTip.Font = new Font("Calibri", 9);
                markerEnd.Tag = new ScenarioInfo() {
                    Id = scenarioId,
                    Name = scenarioName
                };
                markerEnd.ToolTipMode = MarkerTooltipMode.Always;
                overlay.Markers.Add(markerEnd);
            }
        }

        private void ScenarioSelectClick(object sender, RoutedEventArgs e)
        {
            ScenarioSelectClicked?.Invoke(this, new EventsArgs<object>(null));
        }

        public event EventsHandler<object> ScenarioSelectClicked;

        public event EventsHandler<ScenarioInfo> Navigated;

        public struct ScenarioInfo
        {
            public string Id { get; set; }
            public string Name { get; set; }

            public override int GetHashCode()
            {
                return Id.GetHashCode() ^ Name.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                return obj is ScenarioInfo && obj.GetHashCode() == GetHashCode();
            }
        }

        public struct PointDate
        {
            public PointLatLng Point { get; set; }
            public DateTime DateTime { get; set; }
        }

        public class GeolocationScenarioHistoryView
        {
            public ScenarioInfo ScenarioInfo { get; set; }
            public GeolocationData[] Datas { get; set; }
        }
    }
}