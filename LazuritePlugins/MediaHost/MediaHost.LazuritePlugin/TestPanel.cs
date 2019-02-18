using MediaHost.Bases;
using System.Windows.Media;

namespace MediaHost.LazuritePlugin
{
    public class TestPanel : MediaPanelBase
    {
        public TestPanel(string elementName) : base(elementName)
        {
            Background = Brushes.Black;
        }
    }
}