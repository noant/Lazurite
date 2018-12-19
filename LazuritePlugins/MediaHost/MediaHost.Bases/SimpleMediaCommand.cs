using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaHost.Bases
{
    public class SimpleMediaCommand : MediaCommandBase
    {
        private readonly Action _execute;
        private readonly Action<string> _executeWithParam;
        private readonly Func<string> _needCurrent;
        private readonly bool _needActivatePanel;

        public SimpleMediaCommand(Action execute, Action<string> executeWithParam, string name, bool allowParam, Func<string> needCurrent, bool needActivatePanel = false)
        {
            AllowParam = allowParam;
            Name = name;
            _execute = execute;
            _executeWithParam = executeWithParam;
            _needCurrent = needCurrent;
            _needActivatePanel = needActivatePanel;
        }

        public SimpleMediaCommand(Action execute, string name, bool needActivatePanel = false):
            this(execute, null, name, false, null, needActivatePanel)
        {

        }

        public SimpleMediaCommand(Action<string> execute, string name, Func<string> needCurrent, bool needActivatePanel = false) :
            this(null, execute, name, true, needCurrent, needActivatePanel)
        {

        }

        public override bool ActivateWithPanelBase(string param = null) => _needActivatePanel;

        public override void Execute()
        {
            _execute?.Invoke();
        }

        public override void Execute(string param)
        {
            _executeWithParam?.Invoke(param);
        }

        public override string Current => _needCurrent?.Invoke() ?? string.Empty;
    }
}
