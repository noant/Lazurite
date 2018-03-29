using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazuriteUI.Windows.Main.Statistics
{
    public class StatisticsFilter
    {
        public static readonly StatisticsFilter Empty = new StatisticsFilter();

        public string[] ScenariosIds { get; set; }
    }
}
