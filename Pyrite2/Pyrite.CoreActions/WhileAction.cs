using Pyrite.ActionsDomain;
using Pyrite.MainDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Pyrite.ActionsDomain.ValueTypes;
using Pyrite.ActionsDomain.Attributes;

namespace Pyrite.CoreActions
{
    [VisualInitialization]
    [OnlyExecute]
    [SuitableValueTypes(typeof(ButtonValueType))]
    [HumanFriendlyName("Пока")]
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
        public ActionsDomain.ValueTypes.AbstractValueType ValueType
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
        
        public void UserInitializeWith<T>() where T : AbstractValueType
        {
            //
        }
    }
}
