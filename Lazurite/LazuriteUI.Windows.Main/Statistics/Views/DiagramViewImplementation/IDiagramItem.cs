using Lazurite.MainDomain.Statistics;
using System;
using System.Windows.Media;

namespace LazuriteUI.Windows.Main.Statistics.Views.DiagramViewImplementation
{
    public interface IDiagramItem
    {
        void SetColors(SolidColorBrush mainColor, SolidColorBrush scaleColor);
        double Zoom { get; set; }
        DateTime? MaxDateCurrent { get; }
        DateTime? MinDateCurrent { get; }
        int Scroll { get; set; }
        DateTime MaxDate { get; set; }
        DateTime MinDate { get; set; }
        StatisticsItem GetItemNear(DateTime dateTime);
        void SelectPoint(DateTime dateTime);
        ScenarioStatistic Points { get; set; }
        void Refresh();
        bool RequireLarge { get; }
    }
}
