using Lazurite.ActionsDomain;
using Lazurite.ActionsDomain.Attributes;
using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.CoreActions.CoreActions;
using Lazurite.MainDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lazurite.CoreActions
{
    [OnlyExecute]
    [VisualInitialization]
    [HumanFriendlyName("Выполнить")]
    [SuitableValueTypes(typeof(ButtonValueType))]
    public class ExecuteAction : IMultipleAction, IAction
    {
        public string Caption
        {
            get
            {
                if (InputValue != null)
                {
                    return ActionsDomain.Utils.ExtractHumanFriendlyName(MasterActionHolder.GetType()) + " " + MasterActionHolder.Action.Caption + " = " +
                        ActionsDomain.Utils.ExtractHumanFriendlyName(InputValue.GetType()) + " " + InputValue.Action.Caption;
                }
                else
                {
                    return ActionsDomain.Utils.ExtractHumanFriendlyName(MasterActionHolder.GetType()) + " " + MasterActionHolder.Action.Caption;
                }
            }
            set
            {
                //
            }
        }

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

        public ValueTypeBase ValueType
        {
            get;
            set;
        } = new ButtonValueType();

        public IAction[] GetAllActionsFlat()
        {
            return new[] { MasterActionHolder.Action, InputValue.Action };
        }

        public void Initialize()
        {
            //do nothing
        }

        public void UserInitialize()
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
            MasterActionHolder.Action.SetValue(context, InputValue?.Action.GetValue(context));
        }

        private ActionHolder _actionHolder = new ActionHolder();
        public ActionHolder MasterActionHolder {
            get
            {
                return _actionHolder;
            }
            set
            {
                if (_inputValue != null && !_actionHolder.Action.ValueType.IsCompatibleWith(_inputValue.Action.ValueType))
                    _inputValue.Action = Utils.Default(_inputValue.Action.ValueType);
                _actionHolder = value;
            }
        }

        private ActionHolder _inputValue = new ActionHolder();
        public ActionHolder InputValue {
            get
            {
                return _inputValue;
            }
            set
            {
                if (_actionHolder == null)
                    throw new InvalidOperationException("Cannot set InputValue if Action is null");
                
                _inputValue = value;
            }
        }
        
        public event ValueChangedDelegate ValueChanged;
    }
}