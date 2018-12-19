using MediaHost.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MediaHost.LazuritePlugin
{
    public class TestPanel : MediaPanelBase
    {
        public TestPanel(string elementName) : base(elementName)
        {
            Background = Brushes.Beige;
        }
    }
}
