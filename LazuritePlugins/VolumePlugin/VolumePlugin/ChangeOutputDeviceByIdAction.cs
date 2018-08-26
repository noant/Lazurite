using Lazurite.ActionsDomain;
using Lazurite.ActionsDomain.Attributes;
using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.Shared.ActionCategory;
using LazuriteUI.Icons;

namespace VolumePlugin
{
    [LazuriteIcon(Icon.Sound3)]
    [HumanFriendlyName("Устройство воспроизведения (список)")]
    [SuitableValueTypes(typeof(StateValueType))]
    [Category(Category.Multimedia)]
    public class ChangeOutputDeviceByIdAction : IAction
    {
        public string Caption
        {
            get => string.Empty;

            set
            {
                //do nothing
            }
        }

        public bool IsSupportsEvent => false;

        public bool IsSupportsModification => false;

        public ValueTypeBase ValueType
        {
            get
            {
                return new StateValueType()
                {
                    AcceptedValues = Utils.GetDevices()
                };
            }
            set { }
        }

        public event ValueChangedEventHandler ValueChanged;

        public string GetValue(ExecutionContext context) => 
            Utils.GetDefaultOutputDeviceName();

        public void Initialize()
        {
            //do nothing
        }

        public void SetValue(ExecutionContext context, string value) => 
            Utils.SetPlaybackDevice(value);

        public bool UserInitializeWith(ValueTypeBase valueType, bool inheritsSupportedValues) => true;
    }
}
