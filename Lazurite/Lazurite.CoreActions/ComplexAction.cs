﻿using Lazurite.ActionsDomain;
using Lazurite.MainDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.ActionsDomain.Attributes;

namespace Lazurite.CoreActions
{
    [OnlyExecute]
    [VisualInitialization]
    [SuitableValueTypes(typeof(ButtonValueType))]
    [HumanFriendlyName("Составное действие")]
    public class ComplexAction : IAction, IMultipleAction
    {
        public ComplexAction()
        {
            Actions = new List<IAction>();
        }

        public List<IAction> Actions { get; set; }

        public bool IsSupportsEvent
        {
            get
            {
                return ValueChanged != null;
            }
        }

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
            foreach (var action in Actions)
            {
                if (context.CancellationToken.IsCancellationRequested)
                    break;
                action.SetValue(context, string.Empty);
            }
        }

        public event ValueChangedDelegate ValueChanged;
    }
}