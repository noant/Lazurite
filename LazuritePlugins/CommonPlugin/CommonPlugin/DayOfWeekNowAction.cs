using Lazurite.ActionsDomain;
using Lazurite.ActionsDomain.Attributes;
using Lazurite.ActionsDomain.ValueTypes;
using LazuriteUI.Icons;
using System;
using System.Linq;

namespace CommonPlugin
{
    [OnlyGetValue]
    [HumanFriendlyName("День недели сейчас")]
    [SuitableValueTypes(typeof(StateValueType))]
    [LazuriteIcon(Icon.CalendarDay)]
    public class DayOfWeekNowAction : IAction
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
        } = new StateValueType() {
            AcceptedValues = new[] { "Понедельник", "Вторник", "Среда", "Четверг", "Пятница", "Суббота", "Воскресенье" }
        };

        public event ValueChangedEventHandler ValueChanged;

        public string GetValue(ExecutionContext context)
        {
            if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
                return ValueType.AcceptedValues.Last();
            return ValueType.AcceptedValues[(int)DateTime.Now.DayOfWeek+1];
        }

        public void Initialize()
        {
            //
        }

        public void SetValue(ExecutionContext context, string value)
        {
            //
        }

        public bool UserInitializeWith(ValueTypeBase valueType, bool inheritsSupportedValues)
        {
            return true;
        }
    }
}
