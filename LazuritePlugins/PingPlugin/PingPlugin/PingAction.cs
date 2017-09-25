using Lazurite.ActionsDomain;
using Lazurite.ActionsDomain.Attributes;
using LazuriteUI.Icons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lazurite.ActionsDomain.ValueTypes;
using System.Net.NetworkInformation;
using PingPluginUtils;
using PingPluginUI;

namespace PingPlugin
{
    [OnlyGetValue]
    [LazuriteIcon(Icon.TransitConnection)]
    [HumanFriendlyName("Устройство в сети")]
    [SuitableValueTypes(typeof(ToggleValueType))]
    public class PingAction : IAction, IPingAction
    {
        public string Host { get; set; }

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

        public event ValueChangedDelegate ValueChanged;

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
