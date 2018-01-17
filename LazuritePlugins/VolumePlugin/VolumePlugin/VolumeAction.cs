using Lazurite.ActionsDomain;
using Lazurite.ActionsDomain.Attributes;
using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.Shared.ActionCategory;
using LazuriteUI.Icons;

namespace VolumePlugin
{
    [LazuriteIcon(Icon.Sound3)]
    [HumanFriendlyName("Уровень звука")]
    [SuitableValueTypes(typeof(FloatValueType))]
    [Category(Category.Multimedia)]
    public class VolumeAction : IAction
    {
        public string Caption
        {
            get
            {
                return string.Empty;
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
        } = new FloatValueType() { AcceptedValues = new string[] { 0.ToString(), 100.ToString() } };

        public event ValueChangedEventHandler ValueChanged;

        public string GetValue(ExecutionContext context)
        {
            return Utils.GetVolumeLevel().ToString();
        }

        public void Initialize()
        {
            //do nothing
        }

        public void SetValue(ExecutionContext context, string value)
        {
            Utils.SetVolumeLevel(double.Parse(value));
        }

        public bool UserInitializeWith(ValueTypeBase valueType, bool inheritsSupportedValues)
        {
            return true;
        }
    }
}
