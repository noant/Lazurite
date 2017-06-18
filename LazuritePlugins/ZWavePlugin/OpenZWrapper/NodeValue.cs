using OpenZWaveDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenZWrapper
{
    public class NodeValue
    {
        public NodeValue(ZWValueID id, Node node)
        {
            Source = id;
            Node = node;
            Id = id.GetId();
            ZWValueType = id.GetType();
            ValueType = (ValueType)(int)ZWValueType;
            Genre = (ValueGenre)(int)id.GetGenre();
        }

        public void Refresh()
        {
            var manager = Node.Manager;
            this.Description = manager.GetValueHelp(Source);
            this.Name = manager.GetValueLabel(Source);
            this.Unit = manager.GetValueUnits(Source);
            if (this.ValueType == ValueType.List)
            {
                var possibleValues = new string[0];
                if (manager.GetValueListItems(Source, out possibleValues))
                    PossibleValues = possibleValues;
            }
            InternalSet(Helper.GetValue(manager, Source, ZWValueType, PossibleValues));
        }
        
        public ZWValueID Source { get; private set; }
        public Node Node { get; private set; }
        internal ZWValueID.ValueType ZWValueType { get; private set; }
        public ValueType ValueType { get; private set; }

        public string[] PossibleValues { get; private set; }

        public decimal Max { get; set; } = decimal.MaxValue;
        public decimal Min { get; set; } = decimal.MinValue;

        public event Action<object, NodeValueChangedEventArgs> Changed;

        private object _current;
        public object Current {
            get {
                return _current;
            }
            set {
                if (Helper.SetValueSucceed(Node.Manager, Source, ZWValueType, value))
                    InternalSet(value);                
            }
        }

        public ulong Id { get; private set; }
        public ValueGenre Genre { get; private set; }
        public string Description { get; private set; }
        public string Name { get; private set; }
        public string Unit { get; private set; }

        public byte CurrentGroupIdx { get; internal set; }
        public byte CurrentByte { get; internal set; }

        internal void InternalSet(object value)
        {
            _current = value;
            Changed?.Invoke(this,
                new NodeValueChangedEventArgs() {
                    Value = this
                });
        }
    }
}
