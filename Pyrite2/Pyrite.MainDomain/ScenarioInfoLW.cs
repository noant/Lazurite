using Pyrite.ActionsDomain.ValueTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Pyrite.MainDomain
{
    [DataContract]
    public class ScenarioInfoLW
    {
        [DataMember]
        public string ScenarioId { get; set; } //guid

        [DataMember]
        public string CurrentValue { get; set; }
    }
}
