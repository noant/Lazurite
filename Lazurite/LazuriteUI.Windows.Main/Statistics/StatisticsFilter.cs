namespace LazuriteUI.Windows.Main.Statistics
{
    public class StatisticsFilter
    {
        public static readonly StatisticsFilter Empty = new StatisticsFilter() { All = true };

        public bool All { get; set; } = false;
        public string[] ScenariosIds { get; set; }
    }
}
