using Pyrite.ActionsDomain.ValueTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Pyrite.MainDomain
{
    [DataContract]
    [KnownType(typeof(ActionsDomain.ValueTypes.ButtonValueType))]
    [KnownType(typeof(ActionsDomain.ValueTypes.DateTimeValueType))]
    [KnownType(typeof(ActionsDomain.ValueTypes.FloatValueType))]
    [KnownType(typeof(ActionsDomain.ValueTypes.ImageValueType))]
    [KnownType(typeof(ActionsDomain.ValueTypes.InfoValueType))]
    [KnownType(typeof(ActionsDomain.ValueTypes.StateValueType))]
    [KnownType(typeof(ActionsDomain.ValueTypes.ToggleValueType))]
    public class ScenarioInfo
    {
        [DataMember]
        public string ScenarioId { get; set; } //guid

        [DataMember]
        public string CurrentValue { get; set; }

        [DataMember]
        public ValueTypeBase ValueType { get; set; }

        [DataMember]
        public UserVisualSettings VisualSettings { get; set; }
    }
}
