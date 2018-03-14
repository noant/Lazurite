using Lazurite.ActionsDomain;
using Lazurite.ActionsDomain.Attributes;
using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.Shared.ActionCategory;
using LazuriteUI.Icons;
using PingPluginUI;
using PingPluginUtils;
using System.Net.NetworkInformation;

namespace PingPlugin
{
    [OnlyGetValue]
    [LazuriteIcon(Icon.TransitConnection)]
    [HumanFriendlyName("Устройство в сети")]
    [SuitableValueTypes(typeof(ToggleValueType))]
    [Category(Category.Administration)]
    public class PingAction : IAction, IPingAction
    {
        public string Host { get; set; } = "127.0.0.1";

        public string Caption
        {
            get
            {
                return string.Format("[{0}]", this.Host);
            }

            set
            {
                //do nothing
            }
        }

        public bool IsSupportsEvent
        {
            get
            {
                return false;
            }
        }

        public bool IsSupportsModification
        {
            get
            {
                return true;
            }
        }

        public ValueTypeBase ValueType
        {
            get;
            set;
        } = new ToggleValueType();

        public event ValueChangedEventHandler ValueChanged;

        public string GetValue(ExecutionContext context)
        {
            if (string.IsNullOrEmpty(Host))
                return ToggleValueType.ValueOFF;

            string success = ToggleValueType.ValueOFF;

            try
            {
                var ping = new Ping();
                if (ping.Send(this.Host).Status == IPStatus.Success)
                    success = ToggleValueType.ValueON;
            }
            catch
            {
                success = ToggleValueType.ValueOFF;
            }

            return success;
        }

        public void Initialize()
        {
            //do nothing
        }

        public void SetValue(ExecutionContext context, string value)
        {
            //do nothing
        }

        public bool UserInitializeWith(ValueTypeBase valueType, bool inheritsSupportedValues)
        {
            var window = new MainWindow(this);
            return window.ShowDialog() ?? false;
        }
    }
}
