using Lazurite.ActionsDomain.ValueTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lazurite.Shared
{
    /// <summary>
    /// Слепок сценария для использования в плагинах
    /// </summary>
    public class ScenarioCast
    {
        private Action<string> _set;
        private Func<string> _get;
        
        public ScenarioCast(Action<string> set, Func<string> get, ValueTypeBase valueType, string name)
        {
            _set = set;
            _get = get;
            ValueType = valueType;
            Name = name;
        }

        public string Value
        {
            get => _get();
            set => _set(value);
        }

        public string Name { get; private set; }
        public ValueTypeBase ValueType { get; private set; }
    }
}
