using Lazurite.ActionsDomain;
using Lazurite.ActionsDomain.Attributes;
using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.Shared;
using Lazurite.Shared.ActionCategory;
using LazuriteUI.Icons;
using OpenZWrapper;
using System;
using System.Linq;
using ZWPluginUI;

namespace ZWavePlugin
{
    [HumanFriendlyName("ZWave устройство")]
    [SuitableValueTypes(
        typeof(StateValueType), typeof(InfoValueType), typeof(FloatValueType), 
        typeof(ButtonValueType), typeof(ToggleValueType))]
    [LazuriteIcon(Icon.ManSensor)]
    [Category(Category.Control)]
    public class ZWaveNodeValue : IAction, IDisposable
    {
        public byte NodeId { get; set; }
        public uint HomeId { get; set; }
        public ulong ValueId { get; set; }
        
        public string Caption
        {
            get
            {
                try
                {
                    var nodeValue = TryGetNodeValue();
                    return nodeValue.Node.FullName + " -> " + nodeValue.Name + " (Index=" + nodeValue.Index + ")";
                }
                catch
                {
                    return "[узел не найден]";
                }
            }
            set { }
        }
        
        public ValueTypeBase ValueType { get; set; }

        public bool IsSupportsEvent => true;

        public bool IsSupportsModification => true;

        public event ValueChangedEventHandler ValueChanged;

        private NodeValue TryGetNodeValue()
        {
            ZWaveManager.Current.WaitForInitialized();
            var nodes = ZWaveManager.Current.GetNodes();
            var node = nodes.FirstOrDefault(x => x.Id.Equals(NodeId) && x.HomeId == HomeId);
            var nodeValue = node?.Values.FirstOrDefault(x => x.Id.Equals(ValueId));
            if (nodeValue == null)
                throw new InvalidOperationException($"Узел не загружен или не существует. Возможно он будет загружен позднее. HomeID={HomeId}, NodeID={NodeId}, ValueID={ValueId}");
            return nodeValue;
        }

        public string GetValue(ExecutionContext context) => TryGetNodeValue().Current.ToString();

        public void Initialize()
        {
            ZWaveManager.Current.NodeValueChanged += NodeValue_Changed;
        }
        
        private void NodeValue_Changed(object sender, EventsArgs<NodeValue> args)
        {
            if (args.Value.Node.HomeId == HomeId && args.Value.Node.Id == NodeId && args.Value.Id == ValueId)
                ValueChanged?.Invoke(this, args.Value.Current.ToString());
        }

        public void SetValue(ExecutionContext context, string value)
        {
            var nodeValue = TryGetNodeValue();
            if (nodeValue.ValueType == OpenZWrapper.ValueType.Bool)
                nodeValue.Current = value == ToggleValueType.ValueON;
            else if (nodeValue.ValueType == OpenZWrapper.ValueType.Byte ||
                nodeValue.ValueType == OpenZWrapper.ValueType.Decimal ||
                nodeValue.ValueType == OpenZWrapper.ValueType.Int ||
                nodeValue.ValueType == OpenZWrapper.ValueType.Short)
                nodeValue.Current = TranslateNumeric(value, nodeValue.ValueType);
            else
                nodeValue.Current = value;
        }

        private object TranslateNumeric(string value, OpenZWrapper.ValueType valueType)
        {
            if (valueType == OpenZWrapper.ValueType.Byte ||
                valueType == OpenZWrapper.ValueType.Int ||
                valueType == OpenZWrapper.ValueType.Short ||
                valueType == OpenZWrapper.ValueType.Decimal)
            {
                var valueNum = double.Parse(value);
                var range = OpenZWrapper.Utils.GetRangeFor(valueType);
                if (valueType == OpenZWrapper.ValueType.Byte)
                    return (byte)TranslateByMargin(Math.Round(valueNum,0), (byte)range.Min, (byte)range.Max);
                if (valueType == OpenZWrapper.ValueType.Int)
                    return (int)TranslateByMargin(Math.Round(valueNum, 0), (int)range.Min, (int)range.Max);
                if (valueType == OpenZWrapper.ValueType.Short)
                    return (short)TranslateByMargin(Math.Round(valueNum, 0), (short)range.Min, (short)range.Max);
                if (valueType == OpenZWrapper.ValueType.Decimal)
                    return (decimal)TranslateByMargin(valueNum, (double)range.Min, (double)range.Max);
            }
            return value;
        }

        private double TranslateByMargin(double val, double min, double max)
        {
            if (val > max)
                return max;
            if (val < min)
                return min;
            return val;
        }

        public bool UserInitializeWith(ValueTypeBase valueType, bool inheritsSupportedValueTypes)
        {
            var manager = ZWaveManager.Current;
            var window = new MainWindow();

            NodeValue nodeValue = null;
            try
            {
                nodeValue = TryGetNodeValue();
            }
            catch
            {
                //do nothing, node not exist
            }

            window.RefreshWith(manager, nodeValue, (nv) => ZWaveTypeComparability.IsTypesComparable(nv, valueType, inheritsSupportedValueTypes));
            if (window.ShowDialog() ?? false)
            {
                nodeValue = window.GetSelectedNodeValue();
                NodeId = nodeValue.Node.Id;
                HomeId = nodeValue.Node.HomeId;
                ValueId = nodeValue.Id;
                ValueType = ZWaveTypeComparability.CreateValueTypeFromNodeValue(nodeValue);
                Initialize();
                return true;
            }
            else return false;
        }

        public void Dispose()
        {
            ZWaveManager.Current.NodeValueChanged -= NodeValue_Changed;
        }
    }
}
