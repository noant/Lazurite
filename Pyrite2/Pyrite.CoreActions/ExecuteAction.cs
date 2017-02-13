using Pyrite.ActionsDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pyrite.CoreActions
{
    [OnlyExecute]
    public class ExecuteAction : IMultipleAction, IAction
    {
        public string Caption
        {
            get
            {
                return "Выполнить";
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

        private ActionsDomain.ValueType _valueType = new ButtonValueType();
        public ActionsDomain.ValueType ValueType
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
