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

        public void UserInitializeWith(AbstractValueType valueType)
        {
            //do nothing
        }

        public string GetValue(ExecutionContext context)
        {
            return string.Empty;
        }

        public void SetValue(ExecutionContext context, string value)
        {
            Action.SetValue(context, InputValue != null ? InputValue.GetValue(context) : string.Empty);
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

        public ValueChangedDelegate ValueChanged { get; set; }
    }
}