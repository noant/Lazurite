using Lazurite.ActionsDomain;
using Lazurite.ActionsDomain.Attributes;
using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.Shared.ActionCategory;

namespace Lazurite.CoreActions.StandardValueTypeActions
{
    [VisualInitialization]
    [HumanFriendlyName("# Изображение")]
    [SuitableValueTypes(typeof(ImageValueType))]
    [Category(Category.Meta)]
    public class GetImageVTAction : IAction, IStandardValueAction
    {
        public string Caption
        {
            get
            {
                return "Изображение";
            }
            set
            {
                //
            }
        }

        public string Value
        {
            get;
            set;
        } = string.Empty;

        public ActionsDomain.ValueTypes.ValueTypeBase ValueType
        {
            get;
            set;
        } = new ImageValueType();

        public bool IsSupportsModification
        {
            get
            {
                return true;
            }
        }

        public bool IsSupportsEvent
        {
            get
            {
                return false;
            }
        }

        public void Initialize()
        {
            //
        }

        public bool UserInitializeWith(ValueTypeBase valueType, bool inheritsSupportedValues)
        {
            return true;
        }

        public string GetValue(ExecutionContext context)
        {
            return Value;
        }

        public void SetValue(ExecutionContext context, string value)
        {
            Value = value;
        }

        public event ValueChangedEventHandler ValueChanged;
    }
}
