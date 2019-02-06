using Lazurite.MainDomain.Statistics;
using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace Lazurite.Windows.Statistics.Internal
{
    public class StatisticsDataManager
    {
        private readonly string _rootPath = "StatisticsData";

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
                    .SelectMany(x => GetItems(scenarioValueType, scenarioId, path, (byte)x.Day, (byte)x.Month, (ushort)x.Year))
                    .ToArray();
            else return new StatisticsDataItem[0];
        }

        private StatisticsDataItem[] GetItems(string scenarioValueType, string scenarioId, string rootPath, byte day, byte month, ushort year)
        {
            var nowDay = string.Format("{0}.{1}.{2}", day, month, year);
            var path = Path.Combine(rootPath, string.Format("{0}.{1}.{2}", day, month, year));
            StatisticsDataItem[] items;
            var mutexName = scenarioId + scenarioValueType + nowDay;
            using (var mutex = new Mutex(false, mutexName))
            {
                mutex.WaitOne();
                items = File.ReadAllLines(path).Select(x => new StatisticsDataItem(x, day, month, year)).ToArray();
                mutex.ReleaseMutex();
            }
            return items;
        }

        public void SetItem(string scenarioId, string scenarioValueType, StatisticsDataItem dataItem)
        {
            var nowDay = DateTime.Now.ToString("d.M.yyyy");
            var directoryPath = Path.Combine(_rootPath, scenarioId + scenarioValueType);
            var path = Path.Combine(directoryPath, nowDay);
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);
            var mutexName = scenarioId + scenarioValueType + nowDay;
            using (var mutex = new Mutex(false, mutexName))
            {
                mutex.WaitOne();
                File.AppendAllText(path, dataItem.CreateString() + Environment.NewLine);
                mutex.ReleaseMutex();
            }
        }
    }
}