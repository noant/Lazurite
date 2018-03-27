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
            var info = new StatisticsScenarioInfo
            {
                ID = scenarioId,
                Name = scenarioName,
                ValueTypeName = scenarioValueType
            };
            var path = Path.Combine(_rootPath, scenarioId + scenarioValueType);
            if (!Directory.Exists(path) || !Directory.GetFiles(path).Any())
                info.IsEmpty = true;
            else
            {
                var dates =
                    Directory.GetFiles(path)
                    .Select(x => DateTime.Parse(Path.GetFileName(x)));
                info.Since = dates.Min();
                info.To = dates.Max();
            }
            return info;
        }

        public StatisticsDataItem[] GetDataItems(string scenarioId, string scenarioValueType, DateTime since, DateTime to)
        {
            since = since.Date;
            to = to.Date;
            var path = Path.Combine(_rootPath, scenarioId + scenarioValueType);
            if (Directory.Exists(path))
                return
                    Directory.GetFiles(path)
                    .Select(x => DateTime.Parse(Path.GetFileName(x)))
                    .Where(x => x >= since && x <= to)
                    .OrderBy(x => x)
                    .SelectMany(x => GetItems(path, (byte)x.Day, (byte)x.Month, (ushort)x.Year))
                    .ToArray();
            else return new StatisticsDataItem[0];
        }

        private StatisticsDataItem[] GetItems(string rootPath, byte day, byte month, ushort year)
        {
            var path = Path.Combine(rootPath, string.Format("{0}.{1}.{2}", day, month, year));
            return File.ReadAllLines(path).Select(x => new StatisticsDataItem(x, day, month, year)).ToArray();
        }

        public void SetItem(string scenarioId, string scenarioValueType, StatisticsDataItem dataItem)
        {
            var nowDay = DateTime.Now.ToString("dd.M.yyyy");
            var directoryPath = Path.Combine(_rootPath, scenarioId + scenarioValueType);
            var path = Path.Combine(directoryPath, nowDay);
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);
            File.AppendAllText(path, dataItem.CreateString() + Environment.NewLine);
        }
    }
}