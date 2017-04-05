using Pyrite.ActionsDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pyrite.ActionsDomain.ValueTypes;

namespace ZWavePlugin
{
    public class ZWaveNodeValue : IAction
    {
        public string Caption
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public ValueChangedDelegate ValueChanged
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public ValueTypeBase ValueType
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public string GetValue(ExecutionContext context)
        {
            throw new NotImplementedException();
        }

        public void Initialize()
        {
            throw new NotImplementedException();
        }

        public void SetValue(ExecutionContext context, string value)
        {
            throw new NotImplementedException();
        }

        public void UserInitializeWith(ValueTypeBase valueType)
        {
            throw new NotImplementedException();
        }
    }
}
