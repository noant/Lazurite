using Lazurite.MainDomain.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazuriteUI.Windows.Main.Statistics
{
    public interface IStatisticsView
    {
        Func<StatisticsFilter, StatisticsItem[]> NeedItems { get; set; }
    }
}
