using Lazurite.ActionsDomain;
using Lazurite.ActionsDomain.Attributes;
using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.Shared.ActionCategory;
using LazuriteUI.Icons;
using System.Diagnostics;

namespace CommonPlugin
{
    [HumanFriendlyName("Выполнить команду")]
    [LazuriteIcon(Icon.Console)]
    [SuitableValueTypes(typeof(InfoValueType))]
    [OnlyExecute]
    [Category(Category.Administration)]
    public class ExecuteCommandAction : IAction
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
        } = new InfoValueType();

        public event ValueChangedEventHandler ValueChanged;

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
            Process.Start("CMD.exe", "/C " + value);
        }

        public bool UserInitializeWith(ValueTypeBase valueType, bool inheritsSupportedValues)
        {
            return true;
        }
    }
}
