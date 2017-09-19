using Lazurite.ActionsDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lazurite.ActionsDomain.ValueTypes;
using LazuriteUI.Icons;
using System.ComponentModel;
using Lazurite.ActionsDomain.Attributes;
using WakeOnLanUtils;

namespace WakeOnLanPlugin
{
    [LazuriteIcon(Icon.NetworkPort)]
    [DisplayName("Wake-On-Lan")]
    [SuitableValueTypes(typeof(ButtonValueType))]
    public class WakeOnLanAction : IAction, IWakeOnLanAction
    {
        public string Caption
        {
            get
            {
                return MacAddress;
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
        } = new ButtonValueType();

        public event ValueChangedDelegate ValueChanged;

        public string GetValue(ExecutionContext context)
        {
            return string.Empty;
        }

        public void Initialize()
        {
            //do nothing
        }

        public void SetValue(ExecutionContext context, string value)
        {
            UdpClientExt.SendWakeOnLan(MacAddress, TryCount, Port);
        }

        public bool UserInitializeWith(ValueTypeBase valueType, bool inheritsSupportedValues)
        {
            throw new NotImplementedException();
        }

        public string MacAddress { get; set; } = "00:00:00:00:00:00";

        public ushort Port { get; set; } = 9;

        public ushort TryCount { get; set; } = 10;
    }
}