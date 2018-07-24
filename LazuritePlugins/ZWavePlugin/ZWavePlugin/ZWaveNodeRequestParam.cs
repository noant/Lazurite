using Lazurite.ActionsDomain;
using Lazurite.ActionsDomain.Attributes;
using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.Shared.ActionCategory;
using LazuriteUI.Icons;
using System;
using System.Linq;
using ZWPluginUI;

namespace ZWavePlugin
{
    [HumanFriendlyName("ZWave запросить конфигурационный параметр")]
    [SuitableValueTypes(typeof(ButtonValueType))]
    [LazuriteIcon(Icon.Settings)]
    [Category(Category.Control)]
    [OnlyExecute]
    public class ZWaveNodeRequestParam : IAction
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
                    return node.FullName + " -> Index=" + ConfigParam;
                }
                catch
                {
                    return "[узел не найден]";
                }
            }
            set { }
        }

        public ValueTypeBase ValueType { get; set; } = new ButtonValueType();

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
            node.RequestConfigParam(ConfigParam);
        }
        
        public bool UserInitializeWith(ValueTypeBase valueType, bool inheritsSupportedValueTypes)
        {
            var manager = ZWaveManager.Current;
            var window = new MainWindowRequestConfigParam();
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
