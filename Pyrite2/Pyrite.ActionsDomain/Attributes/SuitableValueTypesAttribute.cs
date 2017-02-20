using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pyrite.ActionsDomain.Attributes
{
    /// <summary>
    /// Указывает, какие типы могут быть использованы в IAction
    /// </summary>
    public class SuitableValueTypesAttribute: Attribute
    {
        public SuitableValueTypesAttribute(params Type[] types)
        {
            Types = types;
        }

        public SuitableValueTypesAttribute(bool all = false)
        {
            All = all;
        }

        public Type[] Types
        {
            get;
            private set;
        }

        public bool All { get; private set; }
    }
}
