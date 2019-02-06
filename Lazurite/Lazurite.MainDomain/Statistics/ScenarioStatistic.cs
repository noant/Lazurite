using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lazurite.MainDomain.Statistics
{
    [ProtoContract]
    public class ScenarioStatistic
    {
        [ProtoMember(1)]
        public StatisticsItem[] Statistic { get; set; }
        [ProtoMember(2)]
        public StatisticsScenarioInfo ScenarioInfo { get; set; }
    }
}
