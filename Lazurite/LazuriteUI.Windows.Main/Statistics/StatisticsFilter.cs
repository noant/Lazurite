using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazuriteUI.Windows.Main.Statistics
{
    public class StatisticsFilter
    {
        public static readonly StatisticsFilter Empty = new StatisticsFilter() { All = true };

        public bool All { get; set; } = false;
        public string[] ScenariosIds { get; set; }
    }
}
