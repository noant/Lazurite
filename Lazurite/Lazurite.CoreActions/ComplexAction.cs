using Lazurite.ActionsDomain;
using Lazurite.MainDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.ActionsDomain.Attributes;
using Lazurite.CoreActions.CoreActions;

namespace Lazurite.CoreActions
{
    [OnlyExecute]
    [VisualInitialization]
    [SuitableValueTypes(typeof(ButtonValueType))]
    [HumanFriendlyName("Составное действие")]
    public class ComplexAction : IAction, IMultipleAction
    {
        public ComplexAction()
        {
            ActionHolders = new List<ActionHolder>();
        }

        public List<ActionHolder> ActionHolders { get; set; } = new List<ActionHolder>();

        public bool IsSupportsEvent
        {
            get
            {
                return false;
            }
        }

        public string Caption
        {
            get
            {
                return string.Empty;
            }
            set
            {
                //
            }
        }

        public string Value
        {
            get
            {
                return string.Empty;
            }
            set
            {
                
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

        public IAction[] GetAllActionsFlat()
        {
            var result = ActionHolders
                .Select(x=>x.Action)
                .Union(
                ActionHolders
                .Where(x => x.Action is IMultipleAction)
                .Select(x => ((IMultipleAction)x.Action).GetAllActionsFlat()).SelectMany(x => x)).ToArray();
            return result;
        }

        public void Initialize()
        {
            //do nothing
        }

        public bool UserInitializeWith(ValueTypeBase valueType, bool inheritsSupportedValues)
        {
            return false;
        }

        public string GetValue(ExecutionContext context)
        {
            return string.Empty;
        }

        public void SetValue(ExecutionContext context, string value)
        {
            foreach (var holder in ActionHolders)
            {
                if (context.CancellationToken.IsCancellationRequested)
                    break;
                holder.Action.SetValue(context, string.Empty);
            }
        }

        public event ValueChangedDelegate ValueChanged;
    }
}
