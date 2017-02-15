using Pyrite.ActionsDomain;
using Pyrite.MainDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Pyrite.CoreActions
{
    [VisualInitialization]
    [OnlyExecute]
    public class IfAction : IAction, IMultipleAction, ISupportsCancellation
    {
        public CancellationToken CancellationToken
        {
            get;
            set;
        }

        public ComplexAction ActionIf { get; set; }
        public ComplexAction ActionElse { get; set; }
        public ComplexCheckerAction Checker { get; set; }

        public string Caption
        {
            get
            {
                return "ЕСЛИ";
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
                ActionIf.CancellationToken =
                    ActionElse.CancellationToken =
                    Checker.CancellationToken =
                    this.CancellationToken;

                if (Checker.Evaluate())
                    ActionIf.Value = string.Empty;
                else
                    ActionElse.Value = string.Empty;
            }
        }

        private ButtonValueType _valueType = new ButtonValueType();
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
            return new IAction[] { ActionIf, ActionElse, Checker }
            .Union(ActionIf.GetAllActionsFlat())
            .Union(ActionElse.GetAllActionsFlat())
            .Union(Checker.GetAllActionsFlat())
            .ToArray();
        }

        public void Initialize()
        {
            //
        }

        public void UserInitialize()
        {
            //
        }
    }
}
