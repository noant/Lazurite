using Lazurite.MainDomain.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace LazuriteUI.Windows.Main.Statistics.Views.DiagramViewImplementation
{
    public interface IDiagramItem
    {
        void SetColors(Brush mainColor, Brush scaleColor);
        double Zoom { get; set; }
        DateTime? MaxDateCurrent { get; }
        DateTime? MinDateCurrent { get; }
        int Scroll { get; set; }
        DateTime MaxDate { get; set; }
        DateTime MinDate { get; set; }
        StatisticsItem GetItemNear(DateTime dateTime);
        void SelectPoint(DateTime dateTime);
        void SetPoints(string scenarioName, StatisticsItem[] items);
        void Refresh();
        bool RequireLarge { get; }
    }
}
