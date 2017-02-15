using Pyrite.ActionsDomain;
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
    public class ExecuteAction : IMultipleAction, IAction
    {
        public string Caption
        {
            get
            {
                return ReflectionHelper.ExtractHumanFriendlyName(Action.GetType()) + " " + Action.Caption + " = " +
                    ReflectionHelper.ExtractHumanFriendlyName(InputValue.GetType()) + " " + InputValue.Caption;
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
        public ActionsDomain.ValueTypes.ValueType ValueType
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

        public IAction Action { get; set; }
        public IAction InputValue { get; set; }
    }
}
