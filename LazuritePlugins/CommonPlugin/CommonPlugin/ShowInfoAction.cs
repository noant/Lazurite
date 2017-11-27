using Lazurite.ActionsDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lazurite.ActionsDomain.ValueTypes;
using LazuriteUI.Windows.Main.Journal;
using Lazurite.Windows.Logging;
using Lazurite.ActionsDomain.Attributes;
using LazuriteUI.Icons;

namespace CommonPlugin
{
    [OnlyExecute]
    [HumanFriendlyName("Вывод сообщения")]
    [LazuriteIcon(Icon.Message)]
    [SuitableValueTypes(typeof(InfoValueType))]
    public class ShowInfoAction : IAction
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
            JournalManager.Set(value, WarnType.Info, null, true);
        }

        public bool UserInitializeWith(ValueTypeBase valueType, bool inheritsSupportedValues)
        {
            return true;
        }
    }
}
