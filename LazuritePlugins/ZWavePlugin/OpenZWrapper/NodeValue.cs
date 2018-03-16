using Lazurite.Shared;
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

            var range = Utils.GetRangeFor(ValueType);
            Min = range.Min;
            Max = range.Max;
        }

        public void Refresh()
        {
            var manager = Node.Manager;
            Description = manager.GetValueHelp(Source);
            Name = manager.GetValueLabel(Source);
            Unit = manager.GetValueUnits(Source);
            if (ValueType == ValueType.List)
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

        public decimal Max { get; set; } = 100;
        public decimal Min { get; set; } = 0;
        
        private object _current;
        public object Current {
            get => _current;
            set {
                if (!Helper.SetValueSucceed(Node.Manager, Source, ZWValueType, value, PossibleValues))
                    throw new OperationCanceledException(string.Format("Значение [{0}] не выставлено для параметра [{1}][{2}]", value, this.Name, this.Id));
            }
        }

        public ulong Id { get; private set; }
        public ValueGenre Genre { get; private set; }
        public string Description { get; private set; }
        public string Name { get; private set; }
        public string Unit { get; set; }

        public byte CurrentGroupIdx { get; internal set; }
        public byte CurrentByte { get; internal set; }

        internal void InternalSet(object value)
        {
            _current = value;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode() ^ Node.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return (obj as NodeValue)?.GetHashCode() == GetHashCode();
        }
    }
}
