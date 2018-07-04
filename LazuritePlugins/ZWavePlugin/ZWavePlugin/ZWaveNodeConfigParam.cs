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
    [HumanFriendlyName("ZWave конфигурационный параметр")]
    [SuitableValueTypes(typeof(FloatValueType))]
    [LazuriteIcon(Icon.Settings)]
    [Category(Category.Control)]
    [OnlyExecute]
    public class ZWaveNodeConfigParam : IAction
    {
        public byte NodeId { get; set; }
        public uint HomeId { get; set; }
        public byte ConfigParam { get; set; }

        public string Caption
        {
            get
            {
                try
                {
                    var node = ZWaveManager.Current.GetNodes().FirstOrDefault(x => x.Id == NodeId && x.HomeId == HomeId);
                    return node.ProductName + " (ID=" + node.Id + ") -> p:" + ConfigParam;
                }
                catch
                {
                    return "[узел не найден]";
                }
            }
            set { }
        }

        public ValueTypeBase ValueType { get; set; } = new FloatValueType() {
            AcceptedValues = new[] { int.MinValue.ToString(), int.MaxValue.ToString() }
        };

        public bool IsSupportsEvent => false;

        public bool IsSupportsModification => true;

        public event ValueChangedEventHandler ValueChanged;
        
        public string GetValue(ExecutionContext context) => ValueType.AcceptedValues[0];

        public void Initialize()
        {
            //do nothing
        }

        public void SetValue(ExecutionContext context, string value)
        {
            var node = ZWaveManager.Current.GetNodes().FirstOrDefault(x => x.Id == NodeId && x.HomeId == HomeId);
            if (node == null)
                throw new Exception("Узел не найден");
            if (!node.SetConfigParam(ConfigParam, int.Parse(value)))
                throw new Exception("Невозможно выставить параметр");
        }
        
        public bool UserInitializeWith(ValueTypeBase valueType, bool inheritsSupportedValueTypes)
        {
            var manager = ZWaveManager.Current;
            var window = new MainWindow_ConfigParam();
            var node = manager.GetNodes().FirstOrDefault(x => x.HomeId == HomeId && x.Id == NodeId);
            window.RefreshWith(manager, node, ConfigParam);
            if (window.ShowDialog() ?? false)
            {
                node = window.GetSelectedNode();
                NodeId = node.Id;
                HomeId = node.HomeId;
                ConfigParam = window.GetSelectedParamId();
                return true;
            }
            else return false;
        }
    }
}
