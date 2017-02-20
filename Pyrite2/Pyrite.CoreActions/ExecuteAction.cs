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
                return ActionsDomain.Utils.ExtractHumanFriendlyName(Action.GetType()) + " " + Action.Caption + " = " +
                    ActionsDomain.Utils.ExtractHumanFriendlyName(InputValue.GetType()) + " " + InputValue.Caption;
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
                Action.Value = InputValue.Value;
            }
        }

        private ButtonValueType _valueType = new ButtonValueType();
        public AbstractValueType ValueType
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

        public void UserInitializeWith<T>() where T : AbstractValueType
        {
            //do nothing
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
                if (!_action.ValueType.IsCompatibleWith(value.ValueType))
                    throw new InvalidOperationException("Action ValueType is not compatible with installing value ValueType");
                _inputValue = value;
            }
        }
    }
}