using Pyrite.CoreActions.ComparisonTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Pyrite.Utils;

namespace Pyrite.CoreActions
{
    public static class Utils
    {
        private static IComparisonType[] _comparisonTypes;
        public static IComparisonType[] GetComparisonTypes()
        {
            if (_comparisonTypes == null)
            {
                _comparisonTypes = ReflectionUtils.GetAllOfType(typeof(IComparisonType))
                    .Select(x => (IComparisonType)Activator.CreateInstance(x.AsType())).ToArray();
            }
            return _comparisonTypes;
        }
    }
}
