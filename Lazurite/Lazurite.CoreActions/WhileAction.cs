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
using Lazurite.IOC;

namespace Lazurite.CoreActions
{
    [VisualInitialization]
    [OnlyExecute]
    [SuitableValueTypes(typeof(ButtonValueType))]
    [HumanFriendlyName("Пока")]
    public class WhileAction : IAction, IMultipleAction
    {
        private ISystemUtils _systemUtils = Singleton.Resolve<ISystemUtils>();

        public ComplexAction Action { get; set; } = new ComplexAction();
        public ComplexCheckerAction Checker { get; set; } = new ComplexCheckerAction();
        
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
        
        public ValueTypeBase ValueType
        {
            get;
            set;
        } = new ButtonValueType();

        public bool IsSupportsEvent
        {
            get
            {
                return ValueChanged != null;
            }
        }

        public bool IsSupportsModification
        {
            get
            {
                return true;
            }
        }

        public IAction[] GetAllActionsFlat()
        {
            return new[] { Action, Action }
            .Union(Action.GetAllActionsFlat())
            .Union(Checker.GetAllActionsFlat())
            .ToArray();
        }

        public void Initialize()
        {
            //
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
            while (Checker.Evaluate(context))
            {
                if (context.CancellationToken.IsCancellationRequested)
                    break;
                Action.SetValue(context, string.Empty);
                _systemUtils.Sleep(1, context.CancellationToken);
            }
        }

        public event ValueChangedDelegate ValueChanged;
    }
}
