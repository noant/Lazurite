using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.Shared;
using ProtoBuf;

namespace Lazurite.MainDomain
{
    [ProtoContract]
    public class ScenarioInfo
    {
        private string _currentValue;
        private UserVisualSettings _visualSettings = new UserVisualSettings();

        [ProtoMember(1)]
        public string Name { get; set; }

        [ProtoMember(2)]
        public string ScenarioId { get; set; } //guid

        [ProtoMember(3)]
        public string CurrentValue
        {
            get => _currentValue;
            set
            {
                _currentValue = value;
                ValueChanged?.Invoke(this, new EventsArgs<ScenarioInfo>(this));
            }
        }

        [ProtoMember(4)]
        public ValueTypeBase ValueType { get; set; }

        [ProtoMember(5)]
        public UserVisualSettings VisualSettings {
            get => _visualSettings;
            set => _visualSettings = value ?? _visualSettings;
        }

        [ProtoMember(6)]
        public bool IsAvailable { get; set; }

        [ProtoMember(7)]
        public bool OnlyGetValue { get; set; }

        public event EventsHandler<ScenarioInfo> ValueChanged;
    }
}
