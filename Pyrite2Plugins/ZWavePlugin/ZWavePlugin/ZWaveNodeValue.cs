using Pyrite.ActionsDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pyrite.ActionsDomain.ValueTypes;
using OpenZWrapper;

namespace ZWavePlugin
{
    public class ZWaveNodeValue : IAction
    {
        public byte NodeId { get; set; }
        public byte HomeId { get; set; }
        public ulong ValueId { get; set; }

        private NodeValue _nodeValue;

        public string Caption
        {
            get
            {
                return _nodeValue?.Node.Name + " -> " + _nodeValue?.Name;
            }
            set
            {
                //do nothing
            }
        }

        public ValueChangedDelegate ValueChanged
        {
            get;
            set;
        }

        public ValueTypeBase ValueType
        {
            get;
            set;
        }

        public string GetValue(ExecutionContext context)
        {
            return _nodeValue?.Current.ToString();
        }

        public void Initialize()
        {
            ZWaveManager.Current.WaitForInitialized();
            var nodes = ZWaveManager.Current.GetNodes();
            var node = nodes.FirstOrDefault(x => x.Id.Equals(this.NodeId));
            _nodeValue = node.Values.FirstOrDefault(x => x.Id.Equals(this.ValueId));
            if (_nodeValue != null)
                _nodeValue.Changed += (o, e) => this.ValueChanged?.Invoke(this, _nodeValue.Current.ToString());
        }

        public void SetValue(ExecutionContext context, string value)
        {
            _nodeValue.Current = value;
        }

        public void UserInitializeWith(ValueTypeBase valueType)
        {
            throw new NotImplementedException();
        }
    }
}
