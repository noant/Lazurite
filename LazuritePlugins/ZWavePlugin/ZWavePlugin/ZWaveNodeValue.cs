using Lazurite.ActionsDomain;
using Lazurite.ActionsDomain.Attributes;
using Lazurite.ActionsDomain.ValueTypes;
using LazuriteUI.Icons;
using OpenZWrapper;
using System;
using System.Linq;

namespace ZWavePlugin
{
    [HumanFriendlyName("ZWave устройство")]
    [SuitableValueTypes(
        typeof(StateValueType), typeof(InfoValueType), typeof(FloatValueType), 
        typeof(ButtonValueType), typeof(ToggleValueType))]
    [LazuriteIcon(Icon.ManSensor)]
    public class ZWaveNodeValue : IAction
    {
        public byte NodeId { get; set; }
        public uint HomeId { get; set; }
        public ulong ValueId { get; set; }
        
        private NodeValue _nodeValue;

        public string Caption
        {
            get
            {
                return _nodeValue?.Node.ProductName + " -> " + _nodeValue?.Name + " (ID=" + _nodeValue?.Id + ")";
            }
            set
            {
                //do nothing
            }
        }
        
        public ValueTypeBase ValueType
        {
            get;
            set;
        }

        public bool IsSupportsEvent
        {
            get
            {
                return true;
            }
        }

        public bool IsSupportsModification
        {
            get
            {
                return true;
            }
        }

        public event ValueChangedEventHandler ValueChanged;

        public string GetValue(ExecutionContext context)
        {
            if (_nodeValue != null)
                return _nodeValue.Current.ToString();
            return string.Empty;
        }

        public void Initialize()
        {
            ZWaveManager.Current.WaitForInitialized();
            var nodes = ZWaveManager.Current.GetNodes();
            var node = nodes.FirstOrDefault(x => x.Id.Equals(this.NodeId));
            _nodeValue = node.Values.FirstOrDefault(x => x.Id.Equals(this.ValueId));
            if (_nodeValue != null)
                _nodeValue.Changed += NodeValue_Changed;
        }

        private void NodeValue_Changed(object arg1, NodeValueChangedEventArgs arg2)
        {
            var value = _nodeValue.Current.ToString();
            this.ValueChanged?.Invoke(this, _nodeValue.Current.ToString());
        }

        public void SetValue(ExecutionContext context, string value)
        {
            if (_nodeValue.ValueType == OpenZWrapper.ValueType.Bool)
                _nodeValue.Current = value == ToggleValueType.ValueON;
            else if (_nodeValue.ValueType == OpenZWrapper.ValueType.Byte ||
                _nodeValue.ValueType == OpenZWrapper.ValueType.Decimal ||
                _nodeValue.ValueType == OpenZWrapper.ValueType.Int ||
                _nodeValue.ValueType == OpenZWrapper.ValueType.Short)
                _nodeValue.Current = TranslateNumric(value, _nodeValue.ValueType);
            else
                _nodeValue.Current = value;
        }

        private object TranslateNumric(string value, OpenZWrapper.ValueType valueType)
        {
            if (valueType == OpenZWrapper.ValueType.Byte ||
                valueType == OpenZWrapper.ValueType.Int ||
                valueType == OpenZWrapper.ValueType.Short ||
                valueType == OpenZWrapper.ValueType.Decimal)
            {
                var valueNum = double.Parse(value);
                if (valueType == OpenZWrapper.ValueType.Byte)
                    return (byte)TranslateByMargin(Math.Round(valueNum,0), byte.MinValue, byte.MaxValue);
                if (valueType == OpenZWrapper.ValueType.Int)
                    return (int)TranslateByMargin(Math.Round(valueNum, 0), int.MinValue, int.MaxValue);
                if (valueType == OpenZWrapper.ValueType.Short)
                    return (short)TranslateByMargin(Math.Round(valueNum, 0), short.MinValue, short.MaxValue);
                if (valueType == OpenZWrapper.ValueType.Decimal)
                    return (decimal)TranslateByMargin(valueNum, (double)decimal.MinValue, (double)decimal.MaxValue);
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
            //ValueType = valueType;
            //var manager = ZWaveManager.Current;
            //var parameterSelectView = new NodesValuesComplexView();
            //parameterSelectView.AllowChangeRange = true;
            //parameterSelectView.InitializeWith(
            //    manager, 
            //    _nodeValue?.Node, 
            //    _nodeValue, 
            //    (nodeValue) => ZWaveTypeComparability.IsTypesComparable(nodeValue, valueType, inheritsSupportedValueTypes));
            //var window = new ZWaveSelectionWindow(manager);
            //window.SetPrimaryControl(parameterSelectView);
            //if (window.ShowDialog() ?? false)
            //{
            //    if (_nodeValue != null)
            //        _nodeValue.Changed -= NodeValue_Changed;
            //    _nodeValue = parameterSelectView.SelectedNodeValue;
            //    NodeId = _nodeValue.Node.Id;
            //    HomeId = _nodeValue.Node.HomeId;
            //    ValueId = _nodeValue.Id;
            //    ValueType = ZWaveTypeComparability.CreateValueTypeFromNodeValue(_nodeValue);
            //    _nodeValue.Changed += NodeValue_Changed;
            //    return true;
            //}
            /*else*/ return false;
        }
    }
}
