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
    [OnlyExecute]
    [VisualInitialization]
    public class ComplexAction : IAction, IMultipleAction, ISupportsCancellation
    {
        public ComplexAction()
        {
            Actions = new List<IAction>();
        }

        public List<IAction> Actions { get; set; }

        public string Caption
        {
            get
            {
                return "Комплексное действие";
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
                foreach (var action in Actions)
                {
                    if (CancellationToken.IsCancellationRequested)
                        break;
                    if (action is ISupportsCancellation)
                        ((ISupportsCancellation)action).CancellationToken = this.CancellationToken;
                    action.Value = string.Empty;
                }
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

        public CancellationToken CancellationToken
        {
            get;
            set;
        }

        public IAction[] GetAllActionsFlat()
        {
            return Actions
                .Union(
                Actions
                .Where(x => x is IMultipleAction)
                .Select(x => ((IMultipleAction)x).GetAllActionsFlat()).SelectMany(x => x)).ToArray();
        }

        public void Initialize()
        {
            //do nothing
        }

        public void UserInitialize()
        {
            //do nothing
        }
    }
}
