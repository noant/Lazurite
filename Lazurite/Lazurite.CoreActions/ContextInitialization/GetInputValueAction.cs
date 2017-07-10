using Lazurite.ActionsDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.MainDomain;
using Lazurite.ActionsDomain.Attributes;

namespace Lazurite.CoreActions.ContextInitialization
{
    [OnlyGetValue]
    [SuitableValueTypes(true)]
    [HumanFriendlyName("Входящее значение")]
    [VisualInitialization]
    public class GetInputValueAction : IAction, IContextInitializable
    {
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

        public bool IsSupportsEvent
        {
            get
            {
                return ValueChanged != null;
            }
        }

        public ValueTypeBase ValueType
        {
            get;
            set;
        }
        
        public string GetValue(ExecutionContext context)
        {
            this.ValueType = context.AlgorithmContext.ValueType;
            return context.Input;
        }
        
        public void SetValue(ExecutionContext context, string value)
        {
            //
        }

        public bool UserInitializeWith(ValueTypeBase valueType, bool inheritsSupportedValues)
        {
            return true;
        }

        public void Initialize()
        {
            //
        }

        public void Initialize(IAlgorithmContext algoContext)
        {
            this.ValueType = algoContext.ValueType;
        }

        public event ValueChangedDelegate ValueChanged;
    }
}
