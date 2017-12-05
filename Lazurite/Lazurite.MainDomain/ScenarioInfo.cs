using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.Shared;
using System.Runtime.Serialization;

namespace Lazurite.MainDomain
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
        private string _currentValue;

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string ScenarioId { get; set; } //guid

        [DataMember]
        public string CurrentValue {
            get {
                return _currentValue;
            }
            set {
                _currentValue = value;
                ValueChanged?.Invoke(this, new EventsArgs<ScenarioInfo>(this));
            }
        }

        [DataMember]
        public ValueTypeBase ValueType { get; set; }

        [DataMember]
        public UserVisualSettings VisualSettings { get; set; }

        [DataMember]
        public bool IsAvailable { get; set; }

        [DataMember]
        public bool OnlyGetValue { get; set; }

        public event EventsHandler<ScenarioInfo> ValueChanged;
    }
}
