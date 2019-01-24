using GMap.NET.WindowsForms.Markers;
using System.Drawing;

namespace LazuriteUI.Windows.Main.Statistics.Views.GeolocationViewImplementation
{
    public class MarkersEnumerator
    {
        private static readonly GMarkerGoogleType[] BigMarkerTypes = new GMarkerGoogleType[]
        {
            GMarkerGoogleType.blue_dot,
            GMarkerGoogleType.green_dot,
            GMarkerGoogleType.yellow_dot,
            GMarkerGoogleType.orange_dot,
            GMarkerGoogleType.purple_dot,
            GMarkerGoogleType.red_dot,
        };

        private static readonly GMarkerGoogleType[] SmallMarkerTypes = new GMarkerGoogleType[]
        {
            GMarkerGoogleType.blue_small,
            GMarkerGoogleType.green_small,
            GMarkerGoogleType.yellow_small,
            GMarkerGoogleType.orange_small,
            GMarkerGoogleType.purple_small,
            GMarkerGoogleType.red_small,
        };

        private static readonly Pen[] Pens = new Pen[]
        {
            new Pen(Color.Blue, 3),
            new Pen(Color.Green, 3),
            new Pen(Color.Yellow, 3),
            new Pen(Color.Orange, 3),
            new Pen(Color.Purple, 3),
            new Pen(Color.Red, 3),
        };

        private int _currentIndex = -1;

        public ScenarioMapStyle Next
        {
            get
            {
                _currentIndex++;
                if (_currentIndex == Pens.Length)
                    _currentIndex = 0;
                return new ScenarioMapStyle(
                    BigMarkerTypes[_currentIndex],
                    SmallMarkerTypes[_currentIndex],
                    Pens[_currentIndex]);
            }
        }
    }

    public struct ScenarioMapStyle
    {
        public ScenarioMapStyle(GMarkerGoogleType bigMarker, GMarkerGoogleType smallMarker, Pen stroke)
        {
            BigMarker = bigMarker;
            SmallMarker = smallMarker;
            Stroke = stroke;
        }

        public GMarkerGoogleType BigMarker { get; }
        public GMarkerGoogleType SmallMarker { get; }
        public Pen Stroke { get; }
    }
}
