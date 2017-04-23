using Pyrite.ActionsDomain;
using Pyrite.ActionsDomain.Attributes;
using Pyrite.ActionsDomain.ValueTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pyrite.CoreActions
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
                    return ActionsDomain.Utils.ExtractHumanFriendlyName(Action.GetType()) + " " + Action.Caption + " = " +
                        ActionsDomain.Utils.ExtractHumanFriendlyName(InputValue.GetType()) + " " + InputValue.Caption;
                }
                else
                {
                    return ActionsDomain.Utils.ExtractHumanFriendlyName(Action.GetType()) + " " + Action.Caption;
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

        private ButtonValueType _valueType = new ButtonValueType();
        public ValueTypeBase ValueType
        {
            get
            {
                return _valueType;
            }
            set
            {
                //
            }
        }

        public IAction[] GetAllActionsFlat()
        {
            return new[] { Action, InputValue };
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
            Action.SetValue(context, InputValue?.GetValue(context));
        }

        private IAction _action;
        public IAction Action {
            get
            {
                return _action;
            }
            set
            {
                if (_inputValue != null && !_action.ValueType.IsCompatibleWith(_inputValue.ValueType))
                    _inputValue = null;
                _action = value;
            }
        }

        private IAction _inputValue;
        public IAction InputValue {
            get
            {
                return _inputValue;
            }
            set
            {
                if (_action == null)
                    throw new InvalidOperationException("Cannot set InputValue if Action is null");
                _inputValue = value;
            }
        }

        public event ValueChangedDelegate ValueChanged;
    }
}