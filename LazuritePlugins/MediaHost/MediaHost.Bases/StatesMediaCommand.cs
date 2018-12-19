using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaHost.Bases
{
    public class StatesMediaCommand : MediaCommandBase
    {
        private readonly Action _execute;
        private readonly Action<string> _executeWithParam;
        private readonly Func<string, bool> _needActivatePanel;
        private readonly Func<string[]> _needStates;
        private readonly Func<string> _needCurrentState;

        private StatesMediaCommand(Action execute, Action<string> executeWithParam, string name, bool allowParam, Func<string, bool> needActivatePanel = null)
        {
            AllowParam = allowParam;
            Name = name;
            _execute = execute;
            _executeWithParam = executeWithParam;
            _needActivatePanel = needActivatePanel;
        }

        public StatesMediaCommand(Action<string> execute, Func<string[]> needStates, Func<string> needCurrentState, string name, Func<string, bool> needActivatePanel = null)
        {
            AllowParam = true;
            Name = name;
            _executeWithParam = execute;
            _needStates = needStates;
            _needCurrentState = needCurrentState;
            _needActivatePanel = needActivatePanel;
        }

        public override string Current => _needCurrentState();

        public string[] States => _needStates();

        public override bool ActivateWithPanelBase(string param = null) => _needActivatePanel?.Invoke(param) ?? false;

        public override void Execute() => _execute?.Invoke();

        public override void Execute(string param) => _executeWithParam?.Invoke(param);
    }
}
