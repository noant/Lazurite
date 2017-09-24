using Lazurite.ActionsDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.ActionsDomain.Attributes;
using LazuriteUI.Icons;

namespace VolumePlugin
{
    [LazuriteIcon(Icon.Sound3)]
    [HumanFriendlyName("Устройство воспроизведения")]
    public class ChangeOutputDeviceAction : IAction
    {
        public string Caption
        {
            get
            {
                return "Устройство воспроизведения";
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
                return false;
            }
        }

        public ValueTypeBase ValueType
        {
            get;
            set;
        } = new FloatValueType() {
            AcceptedValues = new [] {"0","100"}
        };

        public event ValueChangedDelegate ValueChanged;

        public string GetValue(ExecutionContext context)
        {
            return Utils.GetDefaultOutputDeviceIndex().ToString();
        }

        public void Initialize()
        {
            //do nothing
        }

        public void SetValue(ExecutionContext context, string value)
        {
            Utils.SetOutputAudioDevice((int)double.Parse(value));
        }

        public bool UserInitializeWith(ValueTypeBase valueType, bool inheritsSupportedValues)
        {
            return true;
        }
    }
}
