using Lazurite.MainDomain.Statistics;
using Lazurite.Windows.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lazurite.Windows.Statistics.Internal
{
    public class StatisticsDataManager
    {
        private string _rootPath = "StatisticsData";

        public StatisticsDataManager()
        {
            _rootPath = Path.Combine(Utils.Utils.GetAssemblyFolder(typeof(StatisticsDataManager).Assembly), _rootPath);
        }

        public bool IsDataEmpty(string scenarioId, string scenarioValueType)
        {
            var path = Path.Combine(_rootPath, scenarioId + scenarioValueType);
            return !Directory.Exists(path) || !Directory.GetFiles(path).Any();
        }

        public StatisticsScenarioInfo GetInfo(string scenarioId, string scenarioName, string scenarioValueType)
        {
            var info = new StatisticsScenarioInfo();
            info.ID = scenarioId;
            info.Name = scenarioName;
            info.ValueTypeName = scenarioValueType;
            var path = Path.Combine(_rootPath, scenarioId + scenarioValueType);
            var dates =
                Directory.GetFiles(path)
                .Select(x => DateTime.Parse(Path.GetFileName(x)));
            info.Since = dates.Min();
            info.To = dates.Max();
            return info;
        }

        public StatisticsDataItem[] GetDataItems(string scenarioId, string scenarioValueType, DateTime since, DateTime to)
        {
            since = since.Date;
            to = to.Date;
            var path = Path.Combine(_rootPath, scenarioId + scenarioValueType);
            return 
                Directory.GetFiles(path)
                .Select(x => DateTime.Parse(Path.GetFileName(x)))
                .Where(x => x >= since && x <= to)
                .OrderBy(x => x)
                .Select(x => Path.Combine(path, string.Format("{0}.{1}.{2}", x.Day, x.Month, x.Year)))
                .SelectMany(x => GetItems(x))
                .ToArray();
        }

        private StatisticsDataItem[] GetItems(string filePath)
        {
            return File.ReadAllLines(filePath).Select(x => new StatisticsDataItem(x)).ToArray();
        }

        public void SetItem(string scenarioId, string scenarioValueType, StatisticsDataItem dataItem)
        {
            var nowDay = DateTime.Now.ToString("dd.MM.yy");
            var path = Path.Combine(_rootPath, scenarioId + scenarioValueType, nowDay);
            if (!Directory.Exists(Path.Combine(_rootPath, scenarioId + scenarioValueType)))
                Directory.CreateDirectory(path);
            File.AppendAllText(path, dataItem.CreateString() + Environment.NewLine);
        }
    }
}
