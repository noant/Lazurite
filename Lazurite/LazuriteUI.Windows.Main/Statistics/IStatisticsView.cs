using Lazurite.MainDomain.Statistics;
using System;

namespace LazuriteUI.Windows.Main.Statistics
{
    public interface IStatisticsView
    {
        Action<StatisticsFilter> NeedItems { get; set; }
        void RefreshItems(StatisticsItem[] items, DateTime since, DateTime to);
    }
}
