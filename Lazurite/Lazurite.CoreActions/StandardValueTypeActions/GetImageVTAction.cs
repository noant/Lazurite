using Lazurite.ActionsDomain;
using Lazurite.ActionsDomain.Attributes;
using Lazurite.ActionsDomain.ValueTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lazurite.CoreActions.StandardValueTypeActions
{
    [VisualInitialization]
    [HumanFriendlyName("Стандартные: изображение")]
    [SuitableValueTypes(typeof(ImageValueType))]
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
                return ValueChanged != null;
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

        public event ValueChangedDelegate ValueChanged;
    }
}
