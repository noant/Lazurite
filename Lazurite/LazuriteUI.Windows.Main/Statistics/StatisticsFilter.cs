using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazuriteUI.Windows.Main.Statistics
{
    public class StatisticsFilter
    {
        public static readonly StatisticsFilter Empty = new StatisticsFilter()
        {
            Since = DateTime.MinValue,
            To = DateTime.MaxValue,
        };

        public string ScenarioId { get; set; }
        public DateTime Since { get; set; }
        public DateTime To { get; set; }
    }
}
