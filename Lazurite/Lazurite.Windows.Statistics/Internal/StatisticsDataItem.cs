using Lazurite.IOC;
using Lazurite.MainDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lazurite.Windows.Statistics.Internal
{
    public class StatisticsDataItem
    {
        private static readonly UsersRepositoryBase UsersRepository = Singleton.Resolve<UsersRepositoryBase>();

        private const char Splitter = '~';
        private const char SplitterReplacer = '?';
        private static char NewLineR = '\r';
        private static char NewLineN = '\n';

        public StatisticsDataItem(string sourceId, string sourceName, string sourceType, string value, byte hour, byte minute, byte second)
        {
            SourceId = sourceId;
            SourceType = sourceType;
            SourceName = 
                sourceName?
                .Replace(Splitter, SplitterReplacer)
                .Replace(NewLineN, ' ')
                .Replace(NewLineR, ' ');
            Value = 
                value?
                .Replace(Splitter, SplitterReplacer)
                .Replace(NewLineN, ' ')
                .Replace(NewLineR, ' ');
            Hour = hour;
            Minute = minute;
            Second = second;
        }

        public StatisticsDataItem(string raw, byte day, byte month, ushort year)
        {
            Day = day;
            Month = month;
            Year = year;

            var splitted = raw.Split(Splitter);
            SourceType = splitted[0];
            Hour = byte.Parse(splitted[1]);
            Minute = byte.Parse(splitted[2]);
            Second = byte.Parse(splitted[3]);
            Value = splitted[4] ?? string.Empty;
            if (splitted.Length > 5)
            {
                SourceId = splitted[5];
                SourceName = splitted[6];
            }
            else
            {
                SourceId = UsersRepository.SystemUser.Id;
                SourceName = UsersRepository.SystemUser.Name;
            }
        }

        public string SourceId { get; private set; }
        public string SourceName { get; private set; }
        public string SourceType { get; private set; }
        public string Value { get; private set; }

        public byte Hour { get; private set; }
        public byte Minute { get; private set; }
        public byte Second { get; private set; }

        public byte Day { get; private set; }
        public byte Month { get; private set; }
        public ushort Year { get; private set; }

        public string CreateString()
        {
            if ((string.IsNullOrEmpty(SourceId) && string.IsNullOrEmpty(SourceId)) ||
                UsersRepository.SystemUser.Id == SourceId)
                return string.Format("{0}~{1}~{2}~{3}~{4}", SourceType, Hour, Minute, Second, Value);
            return string.Format("{0}~{1}~{2}~{3}~{4}~{5}~{6}", SourceType, Hour, Minute, Second, Value, SourceId, SourceName);
        }
    }
}
