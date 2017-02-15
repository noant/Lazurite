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
    public class WhileAction : IAction, IMultipleAction, ISupportsCancellation
    {
        public CancellationToken CancellationToken
        {
            get;
            set;
        }

        public ComplexAction Action { get; set; }
        public ComplexCheckerAction Checker { get; set; }

        public string Caption
        {
            get
            {
                return "ПОКА";
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
                Action.CancellationToken =
                    Checker.CancellationToken =
                    this.CancellationToken;

                while (Checker.Evaluate())
                {
                    if (CancellationToken.IsCancellationRequested)
                        break;
                    Action.Value = string.Empty;
                }
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
            return new[] { Action, Action }
            .Union(Action.GetAllActionsFlat())
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
